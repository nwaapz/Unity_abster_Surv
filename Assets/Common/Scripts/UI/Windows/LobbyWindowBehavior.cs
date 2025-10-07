using OctoberStudio.Audio;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.Save;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OctoberStudio.UI
{
    public class LobbyWindowBehavior : Singleton<LobbyWindowBehavior>   
    {
        [SerializeField] StagesDatabase stagesDatabase;
        [SerializeField] GameObject jsBridge;
        [Space]
        [SerializeField] Image stageIcon;
        [SerializeField] Image lockImage;
        [SerializeField] TMP_Text stageLabel;
        public TMP_Text stageNumberLabel,LeaderBoardCounter;

        [Space]
        [SerializeField] Button PlayWagerButton,PlayFreeButton;
        [SerializeField] Button upgradesButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button charactersButton;
        [SerializeField] Button leftButton;
        [SerializeField] Button rightButton;

        [Space]
        [SerializeField] Sprite playButtonEnabledSprite;
        [SerializeField] Sprite playButtonDisabledSprite;

        [Space]
        [SerializeField] Image continueBackgroundImage;
        [SerializeField] RectTransform contituePopupRect;
        [SerializeField] Button confirmButton;
        [SerializeField] Button cancelButton;

        private StageSave save;

        protected override void Awake()
        {
            base.Awake();
            PlayWagerButton.onClick.AddListener(OnPlayWagerButtonClicked);
            PlayFreeButton.onClick.AddListener(() => {OnPlayDemoButtonClicked();});


            leftButton.onClick.AddListener(DecrementSelectedStageId);
            rightButton.onClick.AddListener(IncremenSelectedStageId);

            confirmButton.onClick.AddListener(ConfirmButtonClicked);
            cancelButton.onClick.AddListener(CancelButtonClicked);
        }
        [SerializeField] Image Lock;
        public void DisableWagerPlay()
        {
            print("disabeling wager play");
            lockImage.gameObject.SetActive(true);
            PlayWagerButton.interactable = false;
            Lock.gameObject.SetActive(true);
            // PlayWagerButton.image.color = Color.grey;
            PlayWagerButton.onClick.RemoveAllListeners();
            PlayWagerButton.onClick.AddListener(Login.Instance.TryConnectWallet);
        }

        public void EnableWagerPlay()
        {
              lockImage.gameObject.SetActive(false);
              PlayWagerButton.interactable = true;
            PlayWagerButton.onClick.RemoveAllListeners();
            PlayWagerButton.onClick.AddListener(OnPlayWagerButtonClicked);
            Lock.gameObject.SetActive(false);
        }

        private void Start()
        {
            save = GameController.SaveManager.GetSave<StageSave>("Stage");

            save.onSelectedStageChanged += InitStage;

            if (save.IsPlaying && GameController.FirstTimeLoaded)
            {
               /// continueBackgroundImage.gameObject.SetActive(true);

               // contituePopupRect.gameObject.SetActive(true);

               //// EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);

                //InitStage(save.SelectedStageId);
            } else
            {
                EventSystem.current.SetSelectedGameObject(PlayWagerButton.gameObject);
                save.SetSelectedStageId(save.MaxReachedStageId);
            }

            GameController.InputManager.onInputChanged += OnInputChanged;
            GameController.InputManager.InputAsset.UI.Settings.performed += OnSettingsInputClicked;
            DisableWagerPlay();
        }

        public void Init(UnityAction onUpgradesButtonClicked, UnityAction onSettingsButtonClicked, UnityAction onCharactersButtonClicked)
        {
            upgradesButton.onClick.AddListener(onUpgradesButtonClicked);
            settingsButton.onClick.AddListener(onSettingsButtonClicked);
            charactersButton.onClick.AddListener(onCharactersButtonClicked);
        }

        public void InitStage(int stageId)
        {
            var stage = stagesDatabase.GetStage(stageId);

            stageLabel.text = stage.DisplayName;
            stageNumberLabel.text = $"Stage {stageId + 1}";
            stageIcon.sprite = stage.Icon;

            if(save.SelectedStageId > save.MaxReachedStageId )
            {
                lockImage.gameObject.SetActive(true);
                PlayWagerButton.interactable = false;
                Lock.gameObject.SetActive(true);
                PlayWagerButton.onClick.RemoveAllListeners();
                PlayWagerButton.onClick.AddListener(Login.Instance.TryConnectWallet);
            } else
            {
                if(PaymentSystem.Instance.HasPaid)
                {
                    lockImage.gameObject.SetActive(false);
                    PlayWagerButton.interactable = true;
                    PlayWagerButton.onClick.RemoveAllListeners();
                    PlayWagerButton.onClick.AddListener(OnPlayWagerButtonClicked);
                    Lock.gameObject.SetActive(false);
                }
               
            }

            leftButton.gameObject.SetActive(!save.IsFirstStageSelected);
            rightButton.gameObject.SetActive(save.SelectedStageId != stagesDatabase.StagesCount - 1);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            EasingManager.DoNextFrame(() => EventSystem.current.SetSelectedGameObject(PlayWagerButton.gameObject));

            GameController.InputManager.onInputChanged += OnInputChanged;
            GameController.InputManager.InputAsset.UI.Settings.performed += OnSettingsInputClicked;
        }

        public void Close()
        {
            gameObject.SetActive(false);

            GameController.InputManager.onInputChanged -= OnInputChanged;
            GameController.InputManager.InputAsset.UI.Settings.performed -= OnSettingsInputClicked;
        }

        public void OnPlayWagerButtonClicked()
        {
            if(!PaymentSystem.Instance.HasPaid)
            {
                DisableWagerPlay();
                return;
            }
            save.IsPlaying = true;
            save.ResetStageData = true;
            save.Time = 0f;
            save.XP = 0f;
            save.XPLEVEL = 0;
            PaymentSystem.Instance.LetIn();
            
        }

        public void LetInAfterSessionGenerated()
        {
            
            GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            GameController.LoadStage();
        }

        public void OnPlayDemoButtonClicked()
        {
            
            save.IsPlaying = true;
            save.ResetStageData = true;
            save.Time = 0f;
            save.XP = 0f;
            save.XPLEVEL = 0;
            PaymentSystem.Instance.NotIn();
            GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            GameController.LoadStage();
        }




        private void IncremenSelectedStageId()
        {
            GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            save.SetSelectedStageId(save.SelectedStageId + 1);

            if (!rightButton.gameObject.activeSelf)
            {
                if (leftButton.gameObject.activeSelf)
                {
                    EventSystem.current.SetSelectedGameObject(leftButton.gameObject);
                } else
                {
                    EventSystem.current.SetSelectedGameObject(PlayWagerButton.gameObject);
                }
            }
        }

        private void DecrementSelectedStageId()
        {
            GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            save.SetSelectedStageId(save.SelectedStageId - 1);

            if (!leftButton.gameObject.activeSelf)
            {
                if (rightButton.gameObject.activeSelf)
                {
                    EventSystem.current.SetSelectedGameObject(rightButton.gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(PlayWagerButton.gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            save.onSelectedStageChanged -= InitStage;
            GameController.InputManager.onInputChanged -= OnInputChanged;
        }

        private void OnSettingsInputClicked(InputAction.CallbackContext context)
        {
            settingsButton.onClick?.Invoke();
        }

        private void ConfirmButtonClicked()
        {
            save.ResetStageData = false;

            GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            GameController.LoadStage();
        }

        private void CancelButtonClicked()
        {
            save.IsPlaying = false;

            continueBackgroundImage.DoAlpha(0, 0.3f).SetOnFinish(() => continueBackgroundImage.gameObject.SetActive(false));
            contituePopupRect.DoAnchorPosition(Vector2.down * 2500, 0.3f).SetEasing(EasingType.SineIn).SetOnFinish(() => contituePopupRect.gameObject.SetActive(false));

            EventSystem.current.SetSelectedGameObject(PlayWagerButton.gameObject);
        }

        private void OnInputChanged(InputType prevInputType, InputType inputType)
        {
            if(prevInputType == InputType.UIJoystick)
            {
                if (continueBackgroundImage.gameObject.activeSelf)
                {
                    EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(PlayWagerButton.gameObject);
                }
            }
        }
    }
}