    using UnityEngine;
    using UnityEngine.UI;

    public class TutoPU : MonoBehaviour,IPullManager
    {
        // ------- Managers -------
        private DataManager _dataMgr;
        
        [SerializeField]
        GameObject tutoPanel;
        [SerializeField]
        Button closeBt;

        private void Awake()
        {
            closeBt.onClick.AddListener(TutoClear);
        }

        void TutoClear()
        {
            Time.timeScale = 1f;
            
            _dataMgr.gameData.isFirstTime = false;
            
            tutoPanel.SetActive(false);
        }

        public void PullUseManager()
        {
            _dataMgr = CoreManager.instance.GetManager<DataManager>();
        }
    }
