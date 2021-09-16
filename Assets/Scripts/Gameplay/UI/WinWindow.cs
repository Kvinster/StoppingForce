using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using System.Collections.Generic;

using SF.Controllers;
using SF.Managers;
using SF.Services;
using SF.Utils;

using TMPro;

namespace SF.Gameplay.UI {
	public sealed class WinWindow : MonoBehaviour {
		const string DescTextTemplate = "Boxes used: {0}\nTotal score: {1}";

		const string LeaderboardSendingScoreText = "Sending score";
		const string LeaderboardLoadingText      = "Loading leaderboard";
		const string LeaderboardErrorText        = "Can't connect to leaderboard";

		public TMP_Text   DescText;
		public Button     ExitButton;
		public GameObject GameCompleteRoot;
		public GameObject NextLevelButtonRoot;
		public Button     NextLevelButton;
		[Header("Leaderboard")]
		public float LeaderboardAnimPeriod = 0.2f;
		[Space]
		public GameObject RecordsRoot;
		public Transform  RecordsParent;
		public GameObject RecordPrefab;
		public GameObject MessageRoot;
		public TMP_Text   MessageText;

		LevelManager _levelManager;

		readonly List<WinWindowLeaderboardRecord> _records = new List<WinWindowLeaderboardRecord>();

		bool IsActive { get; set; }

		public void Show(LevelManager levelManager) {
			_levelManager = levelManager;

			DescText.text = string.Format(DescTextTemplate, levelManager.TotalBoxesUsed,
				Mathf.CeilToInt(levelManager.TotalProgress));

			ExitButton.onClick.AddListener(OnExitClick);
			if ( LevelController.Instance.HasLevel(levelManager.LevelIndex + 1) ) {
				GameCompleteRoot.SetActive(false);
				NextLevelButtonRoot.SetActive(true);
				NextLevelButton.onClick.AddListener(OnNextLevelClick);
			} else {
				GameCompleteRoot.SetActive(true);
				NextLevelButtonRoot.SetActive(false);
			}

			InitLeaderboard(levelManager);

			IsActive = true;

			gameObject.SetActive(true);
		}

		public void Hide() {
			IsActive = false;

			gameObject.SetActive(false);
		}

		void InitLeaderboard(LevelManager levelManager) {
			if ( !PlayFabService.IsLoggedIn ) {
				RecordsRoot.SetActive(false);
				MessageRoot.SetActive(true);
				MessageText.text = LeaderboardErrorText;
				return;
			}

			RecordsRoot.SetActive(false);
			MessageRoot.SetActive(true);
			MessageText.text = LeaderboardSendingScoreText;

			PlayFabService.TrySendScore(levelManager.LevelIndex, Mathf.CeilToInt(levelManager.TotalProgress))
				.Then(() => {
					MessageText.text = LeaderboardLoadingText;
					return UnityContext.Instance.Wait(1f);
				})
				.Then(() => PlayFabService.GetLeaderboard(levelManager.LevelIndex))
				.Then(entries => {
					MessageRoot.SetActive(false);
					RecordsRoot.SetActive(true);
					int i;
					for ( i = 0; i < entries.Count; i++ ) {
						var entry  = entries[i];
						var record = i < _records.Count ? _records[i] : CreateRecord();
						record.Init(entry.Position, entry.PlayFabId, entry.StatValue.ToString());
						record.gameObject.SetActive(true);
					}
					for ( ; i < _records.Count; ++i ) {
						_records[i].gameObject.SetActive(false);
					}
				})
				.Catch(exception => {
					Debug.LogErrorFormat(exception.Message);
					RecordsRoot.SetActive(false);
					MessageRoot.SetActive(true);
					MessageText.text = LeaderboardErrorText;
				});
		}

		WinWindowLeaderboardRecord CreateRecord() {
			var recordGo = Instantiate(RecordPrefab, RecordsParent, false);
			var record = recordGo.GetComponent<WinWindowLeaderboardRecord>();
			Assert.IsTrue(record);
			_records.Add(record);
			return record;
		}

		void OnExitClick() {
			Hide();

			SceneService.LoadMainMenu();
		}

		void OnNextLevelClick() {
			Hide();

			var nextLevelIndex = _levelManager.LevelIndex + 1;
			Assert.IsTrue(LevelController.Instance.HasLevel(nextLevelIndex));
			LevelController.Instance.StartLevel(nextLevelIndex);
			SceneService.LoadLevel(nextLevelIndex);
		}
	}
}
