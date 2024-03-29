namespace DungeonDefence
{ 
	using DevelopersHub.RealtimeNetworking.Client;
	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;
	using System;
	using System.Linq; 

	public class UI_Clan : MonoBehaviour
	{


        [SerializeField] private GameObject _elements = null;
        [SerializeField] private GameObject _profilePanel = null;
        [SerializeField] private GameObject _listPanel = null;
        [SerializeField] private GameObject _createPanel = null;
        [SerializeField] private GameObject _warPanel = null;
        [SerializeField] private Button _closeButton = null;

        [Header("Clans List")]
        [SerializeField] private UI_ClanItem _clansPrefab = null;
        [SerializeField] private RectTransform _clansParent = null;
        [SerializeField] private Button _createButton = null;
        [SerializeField] private Button _nextButton = null;
        [SerializeField] private Button _lastButton = null;
        [SerializeField] private Button _prevButton = null;
        [SerializeField] private Button _firstButton = null;
        [SerializeField] private TextMeshProUGUI _pageText = null;

        [Header("Clans Create")]
        [SerializeField] private Button _createConfirm = null;
        [SerializeField] private Button _createCancel = null;
        [SerializeField] private Image _createBackground = null;
        [SerializeField] private Image _createIcon = null;
        [SerializeField] private TMP_InputField _createName = null;
        [SerializeField] private TextMeshProUGUI _createJoinText = null;
        [SerializeField] private TextMeshProUGUI _createTrophiesText = null;
        [SerializeField] private TextMeshProUGUI _createHallText = null;
        [SerializeField] private Button _createJoinNext = null;
        [SerializeField] private Button _createJoinPrev = null;
        [SerializeField] private Button _createTrophiesNext = null;
        [SerializeField] private Button _createTrophiesPrev = null;
        [SerializeField] private Button _createHallNext = null;
        [SerializeField] private Button _createHallPrev = null;
        [SerializeField] private Button _createPatternNext = null;
        [SerializeField] private Button _createPatternPrev = null;

        [Header("Clan Profile")]
        [SerializeField] private Button _profileJoin = null;
        [SerializeField] private Button _profileLeave = null;
        [SerializeField] private Button _profileClans = null;
        [SerializeField] private Button _profileEdit = null;
        [SerializeField] private Button _profileWar = null;
        [SerializeField] private Button _profileWarHistory = null;
        [SerializeField] private Button _profileWarHistoryClose = null;
        [SerializeField] private GameObject _profileHistoryPanel = null;
        [SerializeField] private Button _profileJoinRequests = null;
        [SerializeField] private Button _profileRequestsClose = null;
        [SerializeField] private GameObject _profileRequestsPanel = null;
        [SerializeField] private UI_ClanJoinRequest _profileRequestPrefab = null;
        [SerializeField] private RectTransform _profileRequestGrid = null;
        [SerializeField] private UI_WarReportItem _profileHistoryPrefab = null;
        [SerializeField] private RectTransform _profileHistoryGrid = null;
        [SerializeField] private TextMeshProUGUI _profileName = null;
        [SerializeField] private Image _profileBackground = null;
        [SerializeField] private Image _profileIcon = null;
        [SerializeField] private UI_ClanMember _profileMemberPrefab = null;
        [SerializeField] private RectTransform _profileMembersGrid = null;

        [Header("Clan War Start")]
        [SerializeField] private GameObject _warNormalPanel = null;
        [SerializeField] private Button _warStart = null;
        [SerializeField] private Button _warNormalBack = null;

        [Header("Clan War Members")]
        [SerializeField] private GameObject _warMembersPanel = null;
        [SerializeField] private Button _warConfirm = null;
        [SerializeField] private TextMeshProUGUI _warMembersCount = null;
        [SerializeField] private Button _warMembersBack = null;
        [SerializeField] private UI_WarMemberSelect _warMemberSelectPrefab = null;
        [SerializeField] private RectTransform _warMemberSelectParent = null;

        [Header("Clan War Search")]
        [SerializeField] private GameObject _warSearchPanel = null;
        [SerializeField] private Button _warCancel = null;
        [SerializeField] private Button _warSearchBack = null;
        [SerializeField] private TextMeshProUGUI _warSearchStarter = null;
        [SerializeField] private TextMeshProUGUI _warSearchCount = null;

        [Header("Clan War Map")]
        [SerializeField] private GameObject _warMapPanel = null;
        [SerializeField] private GameObject _warMapSelectPanel = null;
        [SerializeField] private TextMeshProUGUI _warSelectedName = null;
        [SerializeField] private TextMeshProUGUI _warTimer = null;
        [SerializeField] private Button _warSelectedAttack = null;
        [SerializeField] private Button _warEditLayout = null;
      
        [SerializeField] private TextMeshProUGUI _warClan1Name = null;
        [SerializeField] private TextMeshProUGUI _warClan2Name = null;
        [SerializeField] private Image _warClan1Background = null;
        [SerializeField] private Image _warClan1Icon = null;
        [SerializeField] private Image _warClan2Background = null;
        [SerializeField] private Image _warClan2Icon = null;
        [SerializeField] private Button _warMapBack = null;
        [SerializeField] private Button _warMapEnemy = null;
        [SerializeField] private Button _warMapHome = null;
        [SerializeField] private RectTransform _warMap1 = null;
        [SerializeField] private RectTransform _warMap2 = null;
        [SerializeField] private RectTransform _warMap1Content = null;
        [SerializeField] private RectTransform _warMap2Content = null;
        [SerializeField] private UI_WarMember _warMemberPrefab = null;
        [SerializeField] private GameObject _mapNormalPanel = null;
        [SerializeField] private GameObject _mapReportPanel = null;
        [SerializeField] private TextMeshProUGUI _reportTitle = null;
        [SerializeField] private TextMeshProUGUI _reportStars = null;
        [SerializeField] private TextMeshProUGUI _reportClan1Name = null;
        [SerializeField] private TextMeshProUGUI _reportClan2Name = null;
        [SerializeField] private Image _reportClan1Background = null;
        [SerializeField] private Image _reportClan1Icon = null;
        [SerializeField] private Image _reportClan2Background = null;
        [SerializeField] private Image _reportClan2Icon = null;
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private Button _viewReportButton = null;
        [SerializeField] private RectTransform[] _warMemberMap1Parents = null;
        [SerializeField] private RectTransform[] _warMemberMap2Parents = null;

		[Header("Dungeon")]
		[SerializeField] private GameObject normalLayout = null;
		[SerializeField] private GameObject dungeonLayout = null;
		[SerializeField] private Button _dungeonEditLayout = null;

        [Header("Other")]
        public Sprite[] patterns = null;

        private List<UI_ClanItem> clanItems = new List<UI_ClanItem>();
        private bool editingProfile = false;
        private int playerAttacksCount = 0;
        private static UI_Clan _instance = null; public static UI_Clan instance { get { return _instance; } }
        private bool _active = false; public bool isActive { get { return _active; } }

        private Data.Clan profileClan = null;
        private Data.Clan clanToSave = null;

        private List<UI_WarMember> warMembers = new List<UI_WarMember>();
        private List<UI_WarMemberSelect> warMembersSelect = new List<UI_WarMemberSelect>();
        private List<long> membersInWar = new List<long>();
        private List<UI_WarReportItem> warHistoryItems = new List<UI_WarReportItem>();
        private List<UI_ClanJoinRequest> requestItems = new List<UI_ClanJoinRequest>();
        private int warMemberIconSize = 100;
        [HideInInspector] public UI_WarMember selectedWarMember = null;
        [HideInInspector] public Data.ClanWarData warData = null;
        private bool _isReport = false;

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
            _createButton.onClick.AddListener(CreateOpen);
            _createConfirm.onClick.AddListener(CreateConfirm);
            _createCancel.onClick.AddListener(CreateCancel);
            _createJoinNext.onClick.AddListener(CreateJoinNext);
            _createJoinPrev.onClick.AddListener(CreateJoinPrev);
            _createTrophiesNext.onClick.AddListener(CreateTrophiesNext);
            _createTrophiesPrev.onClick.AddListener(CreateTrophiesPrev);
            _createHallNext.onClick.AddListener(CreateHallNext);
            _createHallPrev.onClick.AddListener(CreateHallPrev);
            _createPatternNext.onClick.AddListener(CreatePatternNext);
            _createPatternPrev.onClick.AddListener(CreatePatternPrev);
            _profileJoin.onClick.AddListener(Join);
            _profileLeave.onClick.AddListener(Leave);
            _profileClans.onClick.AddListener(Clans);
            _profileEdit.onClick.AddListener(Edit);
             _dungeonEditLayout.onClick.AddListener(EditDungeon);
            _warMapEnemy.onClick.AddListener(WarEnemy);
            _warMapHome.onClick.AddListener(WarHome);
            _warNormalBack.onClick.AddListener(WarNormalBack);
            _warSearchBack.onClick.AddListener(WarSearchBack);
            _warMapBack.onClick.AddListener(WarMapBack);
            _profileWar.onClick.AddListener(WarOpen);
            _profileWarHistory.onClick.AddListener(WarHistoryOpen);
            _warStart.onClick.AddListener(WarSearchStart);
            _warCancel.onClick.AddListener(WarSearchCancel);
            _warMembersBack.onClick.AddListener(WarMembersBack);
            _warConfirm.onClick.AddListener(WarConfirm);
            _warEditLayout.onClick.AddListener(EditLayout);
            _warSelectedAttack.onClick.AddListener(Attack);
            _viewReportButton.onClick.AddListener(ViewWarResults);
            _confirmButton.onClick.AddListener(ConfirmWarResults);
            _profileWarHistoryClose.onClick.AddListener(CloseWarHistoryList);
            _profileJoinRequests.onClick.AddListener(RequestsOpen);
            _profileRequestsClose.onClick.AddListener(CloseRequestsList);
        }

        public void Open()
        {
            _active = true;
            _profilePanel.SetActive(false);
            _listPanel.SetActive(false);
            _createPanel.SetActive(false);
            _warPanel.SetActive(false);
            Packet packet = new Packet();
            if (Player.instance.data.clanID > 0)
            {
                packet.Write((int)Player.RequestId.OPENCLAN);
                long id = 0;
                packet.Write(id);
                packet.Write(id);
            }
            else
            {
                profileClan = null;
                packet.Write((int)Player.RequestId.GETCLANS);
                packet.Write(0);
            }
            Sender.TCP_Send(packet);
            _elements.SetActive(true);
        }

        public void Close()
        {
            ClearClanMembers();
            ClearRequestItems();
            ClearWarHistoryItems();
            ClearWarMembersSelect();
            ClearClanItems();
            ClearWarMembers();
            _active = false;
            _elements.SetActive(false);
        }

        public void SelectWarMember(UI_WarMember item)
        {
            if (selectedWarMember != null)
            {
                if (selectedWarMember == item)
                {
                    selectedWarMember.selectedEffects.SetActive(false);
                    _warMapSelectPanel.gameObject.SetActive(false);
                    selectedWarMember = null;
                    return;
                }
                else
                {
                    selectedWarMember.selectedEffects.SetActive(false);
                }
            }
            selectedWarMember = item;
            _warSelectedName.text = selectedWarMember._data.name;
            _warSelectedAttack.gameObject.SetActive(selectedWarMember._data.clanID != Player.instance.data.clanID);
            _warSelectedAttack.interactable = (warData.clan1.war.stage == 2 && !_isReport);
            selectedWarMember.selectedEffects.SetActive(true);
            _warMapSelectPanel.gameObject.SetActive(true);
        }

        private void Attack()
        {
            if (selectedWarMember != null && playerAttacksCount < Data.clanWarAttacksPerPlayer)
            {
                Packet packet = new Packet();
                packet.Write((int)Player.RequestId.WARATTACK);
                packet.Write(selectedWarMember._data.id);
                Sender.TCP_Send(packet);
            }
        }

        public void AttackResponse(long target, Data.OpponentData opponent)
        {
            if (target > 0 && opponent != null)
            {
                bool attack = UI_Battle.instance.Display(opponent.buildings, target, Data.BattleType.dungeon, opponent.dungeonUnits);
                if (attack)
                {
                    UI_Main.instance.SetStatus(false);
                    Close();
                }
                else
                {
                    WarOpen();
                }
            }
            else
            {
                Debug.Log("No target found.");
            }
        }

        private void EditLayout()
        {
            UI_WarLayout.instance.SetStatus(true);
            Close();
        }
        

        public void ClansListOpen(Data.ClansList clans)
        {
            ClearClanItems();
            if (clans.clans.Count > 0)
            {
                for (int i = 0; i < clans.clans.Count; i++)
                {
                    UI_ClanItem item = Instantiate(_clansPrefab, _clansParent);
                    item.Initialize(clans.clans[i]);
                    clanItems.Add(item);
                }
            }
            _createButton.gameObject.SetActive(Player.instance.data.clanID <= 0);
            _nextButton.interactable = (clans.page != clans.pagesCount && clans.clans.Count > 0);
            _lastButton.interactable = (clans.page != clans.pagesCount && clans.clans.Count > 0);
            _prevButton.interactable = (clans.page != 1 && clans.clans.Count > 0);
            _firstButton.interactable = (clans.page != 1 && clans.clans.Count > 0);
            _pageText.text = clans.page.ToString() + "/" + clans.pagesCount.ToString();
            _listPanel.SetActive(true);
            _profilePanel.SetActive(false);
            _createPanel.SetActive(false);
        }

        public void ClanOpen(Data.Clan clan, List<Data.ClanMember> warMembers)
        {
            profileClan = clan;
            _profileJoin.gameObject.SetActive(Player.instance.data.clanID <= 0 && Player.instance.data.clanID != clan.id);
            _profileLeave.gameObject.SetActive(Player.instance.data.clanID > 0 && Player.instance.data.clanID == clan.id);
            _profileEdit.gameObject.SetActive(Player.instance.data.clanID > 0 && Player.instance.data.clanID == clan.id);
            _profileWar.gameObject.SetActive(Player.instance.data.clanID > 0 && Player.instance.data.clanID == clan.id);
            _profileJoin.interactable = (clan.members.Count < Data.clanMaxMembers);
            _profileName.text = profileClan.name;
            _profileIcon.sprite = patterns[profileClan.pattern];
            _profileBackground.color = Tools.HexToColor(profileClan.backgroundColor);
            _profileIcon.color = Tools.HexToColor(profileClan.patternColor);
            _profileRequestsClose.interactable = true;
            _profileJoinRequests.gameObject.SetActive(Player.instance.data.clanID > 0 && Player.instance.data.clanID == clan.id);
            CreateMembersList(clan.members, clan.id);
            _profileRequestsPanel.SetActive(false);
            _profileHistoryPanel.SetActive(false);
            _listPanel.SetActive(false);
            _createPanel.SetActive(false);
            _warPanel.SetActive(false);
            _profilePanel.SetActive(true);
        }

        private List<UI_ClanMember> clanMembers = new List<UI_ClanMember>();

        private void CreateMembersList(List<Data.ClanMember> members, long clanID)
        {
            ClearClanMembers();
            bool haveKickPermission = false;
            if(Player.instance.data.clanID > 0 && Player.instance.data.clanID == clanID)
            {
                for (int i = 0; i < Data.clanRanksWithKickMembersPermission.Length; i++)
                {
                    if (Data.clanRanksWithKickMembersPermission[i] == Player.instance.data.clanRank)
                    {
                        haveKickPermission = true;
                        break;
                    }
                }
            }
            List<Data.ClanMember>  _members = members.OrderBy(p => p.townHallLevel).ToList();
            for (int i = 0; i < _members.Count; i++)
            {
                UI_ClanMember member = Instantiate(_profileMemberPrefab, _profileMembersGrid);
                member.Initialize(_members[i], haveKickPermission);
                clanMembers.Add(member);
            }
        }

        public void kickResponse(long id, int response)
        {
            for (int i = 0; i < clanMembers.Count; i++)
            {
                if(clanMembers[i].id == id)
                {
                    clanMembers[i].kickResponse(response);
                    break;
                }
            }
        }

        public void RequestsOpen()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.JOINREQUESTS);
            Sender.TCP_Send(packet);
        }

        private void CloseRequestsList()
        {
            _profileRequestsClose.interactable = false;
            Open();
        }

        public void OpenRequestsList(List<Data.JoinRequest> requests)
        {
            UI_ClanJoinRequest.active = null;
            ClearRequestItems();
            bool havePermission = false;
            if (Player.instance.data.clanID > 0)
            {
                for (int i = 0; i < Data.clanRanksWithAcceptJoinRequstsPermission.Length; i++)
                {
                    if (Data.clanRanksWithAcceptJoinRequstsPermission[i] == Player.instance.data.clanRank)
                    {
                        havePermission = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < requests.Count; i++)
            {
                UI_ClanJoinRequest request = Instantiate(_profileRequestPrefab, _profileRequestGrid);
                request.Initialize(requests[i], havePermission);
                requestItems.Add(request);
            }
            _profileRequestsPanel.SetActive(true);
        }

        private void CloseWarHistoryList()
        {
            ClearWarHistoryItems();
            _profileHistoryPanel.SetActive(false);
        }

        public void OpenWarHistoryList(List<Data.ClanWarData> reports)
        {
            ClearWarHistoryItems();
            for (int i = 0; i < reports.Count; i++)
            {
                UI_WarReportItem report = Instantiate(_profileHistoryPrefab, _profileHistoryGrid);
                report.Initialize(reports[i]);
                warHistoryItems.Add(report);
            }
            _profileHistoryPanel.SetActive(true);
        }

        public void WarStarted(long id)
        {
            if (_active)
            {
                Player.instance.RushSyncRequest();
                WarOpen();
            }
            MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Clan war started." }, new string[] { "OK" });
        }

        public void WarOpen()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.OPENWAR);
            Sender.TCP_Send(packet);
        }

        public void WarHistoryOpen()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.WARREPORTLIST);
            Sender.TCP_Send(packet);
        }

        public void WarOpen(Data.ClanWarData data, bool isReport = false)
        {
            _isReport = isReport;
            _mapNormalPanel.SetActive(true);
            _mapReportPanel.SetActive(false);
            warData = data;
            ClearWarMembers();
            selectedWarMember = null;
            _warMapSelectPanel.gameObject.SetActive(false);

            _warMapEnemy.gameObject.SetActive(true);
            _warMapHome.gameObject.SetActive(false);

            _listPanel.SetActive(false);
            _createPanel.SetActive(false);
            _profilePanel.SetActive(false);

            _warMembersPanel.SetActive(false);
            _warNormalPanel.SetActive(false);
            _warSearchPanel.SetActive(false);
            _warMapPanel.SetActive(false);

            if (data.id > 0)
            {
                for (int i = 0; i < data.clan1.members.Count; i++)
                {
                    if (data.clan1.members[i].warID == data.id)
                    {
                        if (data.clan1.members[i].warPos >= 0 && data.clan1.members[i].warPos < _warMemberMap1Parents.Length && _warMemberMap1Parents[data.clan1.members[i].warPos] != null)
                        {
                            UI_WarMember member = Instantiate(_warMemberPrefab, _warMemberMap1Parents[data.clan1.members[i].warPos]);

                            int attacks = 0;
                            for (int j = 0; j < data.clan1.war.attacks.Count; j++)
                            {
                                if (data.clan1.war.attacks[j].attacker == data.clan1.members[i].id)
                                {
                                    attacks++;
                                }
                            }

                            Data.ClanWarAttack bestAttack = null;
                            for (int j = 0; j < data.clan2.war.attacks.Count; j++)
                            {
                                if (data.clan2.war.attacks[j].defender == data.clan1.members[i].id)
                                {
                                    if (bestAttack == null || bestAttack.stars < data.clan2.war.attacks[j].stars)
                                    {
                                        bestAttack = data.clan2.war.attacks[j];
                                    }
                                }
                            }

                            if (data.clan1.members[i].id == Player.instance.data.id)
                            {
                                playerAttacksCount = attacks;
                            }

                            member.Initialize(data.clan1.members[i], attacks, bestAttack);
                            RectTransform rect = member.GetComponent<RectTransform>();
                            rect.sizeDelta = new Vector2(warMemberIconSize, warMemberIconSize);
                            warMembers.Add(member);
                        }
                        else
                        {
                            Debug.LogError("There is somthing wrong with positions.");
                        }
                    }
                }
                for (int i = 0; i < data.clan2.members.Count; i++)
                {
                    if (data.clan2.members[i].warID == data.id)
                    {
                        if (data.clan2.members[i].warPos >= 0 && data.clan2.members[i].warPos < _warMemberMap2Parents.Length && _warMemberMap2Parents[data.clan2.members[i].warPos] != null)
                        {
                            UI_WarMember member = Instantiate(_warMemberPrefab, _warMemberMap2Parents[data.clan2.members[i].warPos]);

                            int attacks = 0;
                            for (int j = 0; j < data.clan2.war.attacks.Count; j++)
                            {
                                if (data.clan2.war.attacks[j].attacker == data.clan2.members[i].id)
                                {
                                    attacks++;
                                }
                            }

                            Data.ClanWarAttack bestAttack = null;
                            for (int j = 0; j < data.clan1.war.attacks.Count; j++)
                            {
                                if (data.clan1.war.attacks[j].defender == data.clan2.members[i].id)
                                {
                                    if (bestAttack == null || bestAttack.stars < data.clan1.war.attacks[j].stars)
                                    {
                                        bestAttack = data.clan1.war.attacks[j];
                                    }
                                }
                            }

                            if (data.clan2.members[i].id == Player.instance.data.id)
                            {
                                playerAttacksCount = attacks;
                            }

                            member.Initialize(data.clan2.members[i], attacks, bestAttack);
                            RectTransform rect = member.GetComponent<RectTransform>();
                            rect.sizeDelta = new Vector2(warMemberIconSize, warMemberIconSize);
                            warMembers.Add(member);
                        }
                        else
                        {
                            Debug.LogError("There is somthing wrong with positions.");
                        }
                    }
                }

                _warClan1Name.text = data.clan1.name;
                _warClan1Background.color = Tools.HexToColor(data.clan1.backgroundColor);
                _warClan1Icon.color = Tools.HexToColor(data.clan1.patternColor);
                _warClan1Icon.sprite = patterns[data.clan1.pattern];

                _warClan2Name.text = data.clan2.name;
                _warClan2Background.color = Tools.HexToColor(data.clan2.backgroundColor);
                _warClan2Icon.color = Tools.HexToColor(data.clan2.patternColor);
                _warClan2Icon.sprite = patterns[data.clan2.pattern];

                _warEditLayout.gameObject.SetActive(data.clan1.war.stage == 1);
                _viewReportButton.gameObject.SetActive(isReport);

                if (isReport)
                {
                    _warTimer.text = "War has ended";

                    _reportClan1Name.text = data.clan1.name;
                    _reportClan1Background.color = Tools.HexToColor(data.clan1.backgroundColor);
                    _reportClan1Icon.color = Tools.HexToColor(data.clan1.patternColor);
                    _reportClan1Icon.sprite = patterns[data.clan1.pattern];

                    _reportClan2Name.text = data.clan2.name;
                    _reportClan2Background.color = Tools.HexToColor(data.clan2.backgroundColor);
                    _reportClan2Icon.color = Tools.HexToColor(data.clan2.patternColor);
                    _reportClan2Icon.sprite = patterns[data.clan2.pattern];

                    if (warData.winnerID <= 0)
                    {
                        _reportTitle.text = "Draw";
                        _reportTitle.color = Color.white;
                    }
                    else if (warData.winnerID == Player.instance.data.clanID)
                    {
                        _reportTitle.text = "Vicrory";
                        _reportTitle.color = Color.green;
                    }
                    else
                    {
                        _reportTitle.text = "Defeat";
                        _reportTitle.color = Color.red;
                    }

                    _reportStars.text = warData.clan1Stars + " - " + warData.clan2Stars;
                }
                _mapNormalPanel.SetActive(!_isReport);
                _mapReportPanel.SetActive(_isReport);
                _warMapBack.gameObject.SetActive(!_isReport);
                _warMapPanel.SetActive(true);
            }
            else
            {
                if (data.searching)
                {
                    _warSearchStarter.text = "Search started by " + data.starter;
                    _warSearchCount.text = data.count.ToString() + " VS " + data.count.ToString();
                    _warSearchPanel.SetActive(true);
                }
                else
                {
                    bool found = false;
                    for (int i = 0; i < Data.clanRanksWithWarPermission.Length; i++)
                    {
                        if (Data.clanRanksWithWarPermission[i] == Player.instance.data.clanRank)
                        {
                            found = true;
                            break;
                        }
                    }
                    _warStart.gameObject.SetActive(found);
                    _warNormalPanel.SetActive(true);
                }
            }

            _warMap1.anchoredPosition = Vector2.zero;
            _warMap2.anchoredPosition = new Vector2(Screen.width, 0);
            _warMap1Content.anchoredPosition = Vector2.zero;

            _warPanel.SetActive(true);
        }

        private void ViewWarResults()
        {
            _warMapBack.gameObject.SetActive(false);
            _mapNormalPanel.SetActive(false);
            _mapReportPanel.SetActive(true);
        }

        private void ConfirmWarResults()
        {
            _warMapBack.gameObject.SetActive(true);
            _mapNormalPanel.SetActive(true);
            _mapReportPanel.SetActive(false);
        }

        private void Update()
        {
            if (_active && warData != null && warData.clan1 != null)
            {
                if (warData.clan1.war != null && warData.clan1.war.stage > 0)
                {
                    double seconds = 0;
                    TimeSpan span = Player.instance.data.nowTime - warData.clan1.war.start;
                    if (warData.clan1.war.stage == 1)
                    {
                        if (span.TotalSeconds < (Data.clanWarPrepHours * 3600))
                        {
                            seconds = (Data.clanWarPrepHours * 3600) - span.TotalSeconds;
                            _warTimer.text = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss") + "\n" + "Prep Time";
                        }
                        else
                        {
                            // Prep time is over, it's barrle time
                            warData.clan1.war.stage = 2;
                            WarOpen();
                        }
                    }
                    else
                    {
                        if (span.TotalSeconds < ((Data.clanWarPrepHours + Data.clanWarBattleHours) * 3600))
                        {
                            seconds = ((Data.clanWarPrepHours + Data.clanWarBattleHours) * 3600) - span.TotalSeconds;
                            _warTimer.text = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
                        }
                        else
                        {
                            warData.clan1.war.stage = 0;
                            _warTimer.text = "War has ended";
                            _mapNormalPanel.SetActive(false);
                        }
                    }
                }
            }
        }

        private void WarEnemy()
        {
            _warMapEnemy.gameObject.SetActive(false);
            StartCoroutine(WarEnemy(1f));
        }

        private IEnumerator WarEnemy(float time)
        {
            Vector2 position = _warMap1.anchoredPosition;
            Vector2 taeget = new Vector2(-Screen.width, position.y);
            _warMap2.anchoredPosition = new Vector2(_warMap2.anchoredPosition.x, position.y);
            float timer = 0;
            while (_warMap1.anchoredPosition != taeget)
            {
                _warMap2Content.anchoredPosition = _warMap1Content.anchoredPosition;
                timer += Time.deltaTime; if (timer > time) { timer = time; }
                _warMap1.anchoredPosition = Vector2.Lerp(position, taeget, timer / time);
                _warMap2.anchoredPosition = new Vector2(Screen.width + _warMap1.anchoredPosition.x, position.y);
                yield return null;
            }
            _warMapHome.gameObject.SetActive(true);
        }

        private void WarHome()
        {
            _warMapHome.gameObject.SetActive(false);
            StartCoroutine(WarHome(1f));
        }

        private IEnumerator WarHome(float time)
        {
            Vector2 position = _warMap2.anchoredPosition;
            Vector2 taeget = new Vector2(Screen.width, position.y);
            _warMap1.anchoredPosition = new Vector2(_warMap1.anchoredPosition.x, position.y);
            float timer = 0;
            while (_warMap2.anchoredPosition != taeget)
            {
                _warMap1Content.anchoredPosition = _warMap2Content.anchoredPosition;
                timer += Time.deltaTime; if (timer > time) { timer = time; }
                _warMap2.anchoredPosition = Vector2.Lerp(position, taeget, timer / time);
                _warMap1.anchoredPosition = new Vector2(_warMap2.anchoredPosition.x - Screen.width, position.y);
                yield return null;
            }
            _warMapEnemy.gameObject.SetActive(true);
        }

        private void WarSearchStart()
        {
            membersInWar.Clear();
            ClearWarMembersSelect();
            _warMembersCount.text = "0 VS 0";

            _warNormalPanel.SetActive(false);
            _warSearchPanel.SetActive(false);
            _warMapPanel.SetActive(false);
            for (int i = 0; i < profileClan.members.Count; i++)
            {
                UI_WarMemberSelect member = Instantiate(_warMemberSelectPrefab, _warMemberSelectParent);
                member.Initialize(profileClan.members[i]);
                warMembersSelect.Add(member);
            }

            _warConfirm.interactable = false;
            _warMembersPanel.SetActive(true);
        }

        public void WarMemberStatus(long id, bool isIn)
        {
            if (isIn && !membersInWar.Contains(id))
            {
                membersInWar.Add(id);
            }
            else if (!isIn && membersInWar.Contains(id))
            {
                membersInWar.Remove(id);
            }
            bool found = false;
            for (int i = 0; i < Data.clanWarAvailableCounts.Length; i++)
            {
                if (Data.clanWarAvailableCounts[i] == membersInWar.Count)
                {
                    found = true;
                    break;
                }
            }
            _warConfirm.interactable = found;
            _warMembersCount.text = membersInWar.Count + " VS " + membersInWar.Count;
        }

        private void WarConfirm()
        {
            _warConfirm.interactable = false;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.STARTWAR);
            string membersDara = Data.Serialize<List<long>>(membersInWar);
            packet.Write(membersDara);
            Sender.TCP_Send(packet);
        }

        public void WarStartResponse(int response)
        {
            Player.instance.RushSyncRequest();
            WarOpen();
        }

        private void WarSearchCancel()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.CANCELWAR);
            Sender.TCP_Send(packet);
        }

        public void WarSearchCancelResponse(int response)
        {
            Player.instance.RushSyncRequest();
            WarOpen();
        }

        private void WarMapBack()
        {
            Open();
        }

        private void WarNormalBack()
        {
            Open();
        }

        private void WarSearchBack()
        {
            Open();
        }

        private void WarMembersBack()
        {
            _warMembersPanel.SetActive(false);
            _warNormalPanel.SetActive(true);
        }

        private void Join()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.JOINCLAN);
            packet.Write(profileClan.id);
            Sender.TCP_Send(packet);
        }

        private void Leave()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.LEAVECLAN);
            Sender.TCP_Send(packet);
        }

        private void Clans()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.GETCLANS);
            packet.Write(0);
            Sender.TCP_Send(packet);
        }

        private void Edit()
        {
            CreateOpen(true);
        }
		private void EditDungeon()
        {
			Debug.Log("EditDungeon clicked");
			dungeonLayout.SetActive(true);
			normalLayout.SetActive(false);
            EditLayout();
        }

        private void CreateOpen()
        {
            CreateOpen(false);
        }

        private void CreateOpen(bool edit)
        {
            editingProfile = edit;
            clanToSave = new Data.Clan();
            if (profileClan != null)
            {
                clanToSave.name = profileClan.name;
                clanToSave.minTrophies = profileClan.minTrophies;
                clanToSave.minTownhallLevel = profileClan.minTownhallLevel;
                clanToSave.pattern = profileClan.pattern;
                clanToSave.background = profileClan.background;
                clanToSave.patternColor = profileClan.patternColor;
                clanToSave.backgroundColor = profileClan.backgroundColor;
                clanToSave.joinType = profileClan.joinType;
            }
            else
            {
                clanToSave.name = "";
                clanToSave.minTrophies = 0;
                clanToSave.minTownhallLevel = 1;
                clanToSave.pattern = 0;
                clanToSave.background = 0;
                clanToSave.patternColor = Tools.ColorToHex(Color.black);
                clanToSave.backgroundColor = Tools.ColorToHex(Color.white);
                clanToSave.joinType = Data.ClanJoinType.AnyoneCanJoin;
            }
            _createName.text = clanToSave.name;
            _createTrophiesText.text = clanToSave.minTrophies.ToString();
            _createHallText.text = clanToSave.minTownhallLevel.ToString();
            _createIcon.sprite = patterns[clanToSave.pattern];
            _createBackground.color = Tools.HexToColor(clanToSave.backgroundColor);
            _createIcon.color = Tools.HexToColor(clanToSave.patternColor);
            UpdateCreateJoinType();
            _createPanel.SetActive(true);
            _listPanel.SetActive(false);
            _warPanel.SetActive(false);
            _profilePanel.SetActive(false);
        }

        private void UpdateCreateJoinType()
        {
            switch (clanToSave.joinType)
            {
                case Data.ClanJoinType.AnyoneCanJoin:
                    _createJoinText.text = "Anyone Can Join";
                    break;
                case Data.ClanJoinType.NotAcceptingNewMembers:
                    _createJoinText.text = "Closed";
                    break;
                case Data.ClanJoinType.TakingJoinRequests:
                    _createJoinText.text = "Request Only";
                    break;
            }
        }

        private void CreateConfirm()
        {
            string name = _createName.text.Trim();
            if (!string.IsNullOrEmpty(name) && name.Length >= Data.clanNameMinLength)
            {
                Packet packet = new Packet();
                if (editingProfile)
                {
                    packet.Write((int)Player.RequestId.EDITCLAN);
                }
                else
                {
                    packet.Write((int)Player.RequestId.CREATECLAN);
                }
                packet.Write(name);
                packet.Write(clanToSave.minTrophies);
                packet.Write(clanToSave.minTownhallLevel);
                packet.Write(clanToSave.pattern);
                packet.Write(clanToSave.background);
                packet.Write(clanToSave.patternColor);
                packet.Write(clanToSave.backgroundColor);
                packet.Write((int)clanToSave.joinType);
                Sender.TCP_Send(packet);
            }
            else
            {
                MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Clan name should not be less than " + Data.clanNameMinLength.ToString() + " characters." }, new string[] { "OK" });
            }
        }

        public void CreateResponse(int response)
        {
            switch (response)
            {
                case 1:
                    Player.instance.RushSyncRequest();
                    Close();
                    // Todo: Toast -> Clan created successfully
                    break;
                case 2:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You already are in a clan." }, new string[] { "OK" });
                    break;
                case 3:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You can only join a clan every 24 hours." }, new string[] { "OK" });
                    break;
                case 4:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You don't have enough resources." }, new string[] { "OK" });
                    break;
                default:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Failed to create the clan." }, new string[] { "OK" });
                    break;
            }
        }

        public void EditResponse(int response)
        {
            switch (response)
            {
                case 2:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You don't have permission to edit the clan." }, new string[] { "OK" });
                    break;
                default:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Failed to edit the clan." }, new string[] { "OK" });
                    break;
            }
        }

        public void JoinResponse(int response)
        {
            switch (response)
            {
                case 1:
                    Player.instance.RushSyncRequest();
                    Close();
                    break;
                case 2:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You already are in a clan." }, new string[] { "OK" });
                    break;
                case 3:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You can only join a clan every 24 hours." }, new string[] { "OK" });
                    break;
                case 4:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You don't have minimum requirements." }, new string[] { "OK" });
                    break;
                case 5:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Can not find the clan." }, new string[] { "OK" });
                    break;
                case 6:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Clan capacity is full." }, new string[] { "OK" });
                    break;
                case 7:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Clan is not accepting new members." }, new string[] { "OK" });
                    break;
                case 8:
                case 9:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Request to join the clan has been sent. You have to wait for them to accept your request." }, new string[] { "OK" });
                    break;
                default:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Failed to join the clan." }, new string[] { "OK" });
                    break;
            }
        }

        public void LeaveResponse(int response)
        {
            switch (response)
            {
                case 1:
                    Close();
                    break;
                case 2:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You can not leave during war." }, new string[] { "OK" });
                    break;
                default:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Failed to leave the clan." }, new string[] { "OK" });
                    break;
            }
        }

        private void ErrorConfirm(int layoutIndex, int buttonIndex)
        {
            if (layoutIndex == 1)
            {
                MessageBox.Close();
            }
        }

        private void CreateCancel()
        {
            Open();
        }

        private void CreateJoinNext()
        {
            int i = (int)clanToSave.joinType + 1;
            if (i > 1) { i = -1; }
            if (i < -1) { i = 1; }
            clanToSave.joinType = (Data.ClanJoinType)i;
            UpdateCreateJoinType();
        }

        private void CreateJoinPrev()
        {
            int i = (int)clanToSave.joinType - 1;
            if (i > 1) { i = -1; }
            if (i < -1) { i = 1; }
            clanToSave.joinType = (Data.ClanJoinType)i;
            UpdateCreateJoinType();
        }

        private void CreateTrophiesNext()
        {
            clanToSave.minTrophies += 100;
            if (clanToSave.minTrophies > 5500) { clanToSave.minTrophies = 0; }
            if (clanToSave.minTrophies < 0) { clanToSave.minTrophies = 5500; }
            _createTrophiesText.text = clanToSave.minTrophies.ToString();
        }

        private void CreateTrophiesPrev()
        {
            clanToSave.minTrophies -= 100;
            if (clanToSave.minTrophies > 5500) { clanToSave.minTrophies = 0; }
            if (clanToSave.minTrophies < 0) { clanToSave.minTrophies = 5500; }
            _createTrophiesText.text = clanToSave.minTrophies.ToString();
        }

        private void CreateHallNext()
        {
            clanToSave.minTownhallLevel += 1;
            if (clanToSave.minTownhallLevel > 15) { clanToSave.minTownhallLevel = 1; }
            if (clanToSave.minTownhallLevel < 1) { clanToSave.minTownhallLevel = 15; }
            _createHallText.text = clanToSave.minTownhallLevel.ToString();
        }

        private void CreateHallPrev()
        {
            clanToSave.minTownhallLevel -= 1;
            if (clanToSave.minTownhallLevel > 15) { clanToSave.minTownhallLevel = 1; }
            if (clanToSave.minTownhallLevel < 1) { clanToSave.minTownhallLevel = 15; }
            _createHallText.text = clanToSave.minTownhallLevel.ToString();
        }

        private void CreatePatternNext()
        {
            clanToSave.pattern += 1;
            if (clanToSave.pattern >= patterns.Length) { clanToSave.pattern = 0; }
            if (clanToSave.pattern < 0) { clanToSave.pattern = patterns.Length - 1; }
            _createIcon.sprite = patterns[clanToSave.pattern];
        }

        private void CreatePatternPrev()
        {
            clanToSave.pattern -= 1;
            if (clanToSave.pattern >= patterns.Length) { clanToSave.pattern = 0; }
            if (clanToSave.pattern < 0) { clanToSave.pattern = patterns.Length - 1; }
            _createIcon.sprite = patterns[clanToSave.pattern];
        }

        private void ClearClanItems()
        {
            for (int i = 0; i < clanItems.Count; i++)
            {
                if (clanItems[i])
                {
                    Destroy(clanItems[i].gameObject);
                }
            }
            clanItems.Clear();
        }

        private void ClearClanMembers()
        {
            for (int i = 0; i < clanMembers.Count; i++)
            {
                if (clanMembers[i])
                {
                    Destroy(clanMembers[i].gameObject);
                }
            }
            clanMembers.Clear();
        }

        private void ClearWarMembers()
        {
            for (int i = 0; i < warMembers.Count; i++)
            {
                if (warMembers[i])
                {
                    Destroy(warMembers[i].gameObject);
                }
            }
            warMembers.Clear();
        }

        private void ClearWarMembersSelect()
        {
            for (int i = 0; i < warMembersSelect.Count; i++)
            {
                if (warMembersSelect[i])
                {
                    Destroy(warMembersSelect[i].gameObject);
                }
            }
            warMembersSelect.Clear();
        }

        private void ClearWarHistoryItems()
        {
            for (int i = 0; i < warHistoryItems.Count; i++)
            {
                if (warHistoryItems[i])
                {
                    Destroy(warHistoryItems[i].gameObject);
                }
            }
            warHistoryItems.Clear();
        }

        private void ClearRequestItems()
        {
            for (int i = 0; i < requestItems.Count; i++)
            {
                if (requestItems[i])
                {
                    Destroy(requestItems[i].gameObject);
                }
            }
            requestItems.Clear();
        }

	}
}