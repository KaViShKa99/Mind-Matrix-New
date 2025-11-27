// using UnityEngine;
// using System;
// using System.Collections;

// // Attach to a persistent GameObject (or the same gameObject as GooglePlayManager)
// public class LivesManager : MonoBehaviour
// {
//     public static LivesManager Instance { get; private set; }

//     // Config
//     public int maxLives = 5;
//     public int refillMinutesPerLife = 5; // 5 minutes per life
//     private int refillSecondsPerLife => refillMinutesPerLife * 60;

//     // Events for UI
//     public event Action<int> OnLivesChanged;                // passes current lives
//     public event Action<long> OnTimeToNextLifeUpdated;      // passes seconds until next life (0 if full)
//     public event Action OnLivesFull;

//     // local cached data when cloud not ready
//     private PlayerData localData;

//     private Coroutine ticker;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//             InitializeLocalData();
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void Start()
//     {
//         // If GooglePlayManager exists, subscribe so we can initialize from cloud when it's loaded
//         if (GooglePlayManager.Instance != null)
//         {
//             GooglePlayManager.Instance.OnCloudDataLoaded += OnCloudLoaded;
//         }

//         // Start ticking every 1 second to update UI timers
//         StartTicker();
//     }

//     private void OnDestroy()
//     {
//         if (GooglePlayManager.Instance != null)
//         {
//             GooglePlayManager.Instance.OnCloudDataLoaded -= OnCloudLoaded;
//         }
//         StopTicker();
//     }

//     private void InitializeLocalData()
//     {
//         localData = new PlayerData();
//         // Load from PlayerPrefs if available
//         localData.lives = PlayerPrefs.GetInt("Lives", maxLives);
//         localData.lastLifeUnix = PlayerPrefs.HasKey("LastLifeUnix") ? long.Parse(PlayerPrefs.GetString("LastLifeUnix")) : GetUnixNow();
//     }

//     private void OnCloudLoaded(PlayerData cloudData)
//     {
//         if (cloudData == null) return;

//         // Use cloud data as truth, but still update PlayerPrefs
//         localData = cloudData;

//         // make sure lastLifeUnix has a valid value
//         if (localData.lastLifeUnix == 0)
//             localData.lastLifeUnix = GetUnixNow();

//         SaveLocalToPrefs();
//         RecalculateLivesFromTimestamp();
//         OnLivesChanged?.Invoke(localData.lives);
//     }

//     // Public API -----------------------------------------------------

//     // Try to consume a life. Returns true if consumed.
//     public bool ConsumeLife()
//     {
//         RecalculateLivesFromTimestamp();

//         if (localData.lives <= 0)
//             return false;

//         localData.lives--;
//         if (localData.lives < maxLives && localData.lastLifeUnix == 0)
//         {
//             // start timer from now
//             localData.lastLifeUnix = GetUnixNow();
//         }

//         PersistChanges();
//         OnLivesChanged?.Invoke(localData.lives);
//         return true;
//     }

//     // Add a life (e.g., after watching ad). Returns true if a life was added.
//     public bool AddLifeFromAd()
//     {
//         RecalculateLivesFromTimestamp();

//         if (localData.lives >= maxLives)
//         {
//             // already full
//             return false;
//         }

//         localData.lives++;
//         // if now full, reset timer marker
//         if (localData.lives >= maxLives)
//         {
//             localData.lastLifeUnix = 0; // no timer needed
//             OnLivesFull?.Invoke();
//         }
//         else
//         {
//             // ensure lastLifeUnix is set so timer continues
//             if (localData.lastLifeUnix == 0)
//                 localData.lastLifeUnix = GetUnixNow();
//         }

//         PersistChanges();
//         OnLivesChanged?.Invoke(localData.lives);
//         return true;
//     }

//     // Add life from other sources (give coins etc)
//     public bool AddLifeInstant()
//     {
//         return AddLifeFromAd();
//     }

//     // Request current lives (always recalculates)
//     public int GetLives()
//     {
//         RecalculateLivesFromTimestamp();
//         return localData.lives;
//     }

//     // Seconds until next life (0 if full)
//     public long GetSecondsUntilNextLife()
//     {
//         RecalculateLivesFromTimestamp(); // ensure up-to-date
//         if (localData.lives >= maxLives) return 0;
//         long now = GetUnixNow();
//         long elapsed = now - localData.lastLifeUnix;
//         long secsUntil = refillSecondsPerLife - (elapsed % refillSecondsPerLife);
//         if (secsUntil == refillSecondsPerLife) secsUntil = 0;
//         return secsUntil;
//     }

//     // Force save (useful for pause/quit)
//     public void PersistChanges()
//     {
//         SaveLocalToPrefs();
//         TrySaveToCloud();
//     }

//     // Internal utilities ---------------------------------------------

//     private void RecalculateLivesFromTimestamp()
//     {
//         if (localData == null) InitializeLocalData();

//         if (localData.lives >= maxLives)
//         {
//             // ensure no timer
//             localData.lastLifeUnix = 0;
//             return;
//         }

//         long now = GetUnixNow();
//         long last = localData.lastLifeUnix;
//         if (last == 0)
//         {
//             // not counting down; set last to now to start timer
//             localData.lastLifeUnix = now;
//             SaveLocalToPrefs();
//             return;
//         }

//         long elapsed = now - last;
//         if (elapsed < 0) elapsed = 0;

//         long gained = elapsed / refillSecondsPerLife;
//         if (gained > 0)
//         {
//             int newLives = (int)Math.Min(maxLives, localData.lives + gained);
//             localData.lives = newLives;

//             if (localData.lives >= maxLives)
//             {
//                 localData.lastLifeUnix = 0; // stop timer
//                 OnLivesFull?.Invoke();
//             }
//             else
//             {
//                 // Move the lastLifeUnix forward by number of gained lives * interval
//                 long advanced = gained * refillSecondsPerLife;
//                 localData.lastLifeUnix = last + advanced;
//             }

//             PersistChanges();
//             OnLivesChanged?.Invoke(localData.lives);
//         }
//     }

//     private void SaveLocalToPrefs()
//     {
//         PlayerPrefs.SetInt("Lives", localData.lives);
//         PlayerPrefs.SetString("LastLifeUnix", localData.lastLifeUnix.ToString());
//         PlayerPrefs.Save();
//     }

//     private void TrySaveToCloud()
//     {
//         // attempt to propagate lives into GooglePlayManager's playerData
//         if (GooglePlayManager.Instance != null && GooglePlayManager.Instance.playerData != null)
//         {
//             var pd = GooglePlayManager.Instance.playerData;
//             pd.lives = localData.lives;
//             pd.lastLifeUnix = localData.lastLifeUnix;
//             GooglePlayManager.Instance.SaveGame(pd);
//         }
//     }

//     private long GetUnixNow()
//     {
//         return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
//     }

//     // Ticker for UI updates (fires every second)
//     private void StartTicker()
//     {
//         if (ticker == null)
//             ticker = StartCoroutine(TickCoroutine());
//     }

//     private void StopTicker()
//     {
//         if (ticker != null)
//         {
//             StopCoroutine(ticker);
//             ticker = null;
//         }
//     }

//     private IEnumerator TickCoroutine()
//     {
//         while (true)
//         {
//             RecalculateLivesFromTimestamp();
//             long secs = GetSecondsUntilNextLife();
//             OnTimeToNextLifeUpdated?.Invoke(secs);
//             yield return new WaitForSeconds(1f);
//         }
//     }

//     // Convenience debug / editor methods
// #if UNITY_EDITOR
//     [ContextMenu("Debug Full Lives")]
//     private void DebugFill()
//     {
//         localData.lives = maxLives;
//         localData.lastLifeUnix = 0;
//         PersistChanges();
//         OnLivesChanged?.Invoke(localData.lives);
//     }

//     [ContextMenu("Debug Remove All Lives")]
//     private void DebugEmpty()
//     {
//         localData.lives = 0;
//         localData.lastLifeUnix = GetUnixNow();
//         PersistChanges();
//         OnLivesChanged?.Invoke(localData.lives);
//     }
// #endif
// }
