﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class GamePlayController : MonoBehaviour
{
    // Rank + target constants
    public readonly int ONE = 0;
    public readonly int TWO = 1;
    public readonly int THREE = 2;
    public readonly int FOUR = 3;
    public readonly int SELF = 4;
    public readonly int ALLIES = 5;
    public readonly int ENEMIES = 6;

    public AudioSource shoot;
    public AudioSource heal;
    //holds order of Units
    private Unit[] order;

    //holds currentCharacter
    private Unit currentCharacter;
    //holds currentAbility selected
    private int currentAbility;

    //holds current place in the order
    private int indexOfOrder;

    //holds Units
    private Unit[] allies;
    private Unit[] enemies;

    //Unit Classes
    private Enforcer enforcer;
    private Medic medic;
    private Rifleman rifleman;
    private Engineer engineer;

    //Enemy Unit Classes
    private Freight freightEnemy1, freightEnemy2, freightEnemy3, freightEnemy4;
    private Infected infectedEnemy;
    private MediBot mediBotEnemy;
    private Psychic psychicEnemy;
    private Security securityEnemy;

    //UI Buttons
    public Button[] enemyAttackButtons = new Button[7]; //first 4 are enemy, 5 is self heal, 6 is heal other, 7 is damage all
    public Button[] abilityButtons = new Button[5];

    //what array to enable in pop up attack
    private bool[] ability1EnableArray, ability2EnableArray, ability3EnableArray, ability4EnableArray, ability5EnableArray;

    //Images for UI
    public Sprite[] enforcerAbilityImages = new Sprite[5];
    public Sprite[] medicAbilityImages = new Sprite[5];
    public Sprite[] riflemanAbilityImages = new Sprite[5];
    public Sprite[] engineerAbilityImages = new Sprite[5];
    public Sprite enabledButtonImageEnemy;
    public Sprite enabledButtonImageAlly;
    public Sprite enabledButtonImageHealAlly;
    public Sprite disabledButtonImage;

    //popUp window and components
    public GameObject popUP;
    public Image popUpImage;
    public Text popUpText;

    //holds text of current character
    public Text currentCharacterText;

    //popup victory
    public GameObject popUpVictory;

    //popup victory
    public GameObject popUpFailure;

    //popup victory
    public GameObject popUpHit;

    //popup victory
    public GameObject popUpMiss;

    //popup attack window
    public GameObject popUPAttack;

    //popStepEnemyAttack
    public GameObject stepEnemyAttackPopUp;

    //UI pop up bools
    private bool popUPEnabled = false;
    private bool popUPAttackEnabled = false;

    //current button
    private string currentButtonClicked;

    //get coordinate info about characters on screen
    public GameObject[] characters;

    public GameObject currentCharacterArrow;

    //This is assuming that there will be 4 enemies at
    public Image enemy1Health;
    public Image enemy2Health;
    public Image enemy3Health;
    public Image enemy4Health;

    //This is assuming there will be four ally units
    public Image ally1Health;
    public Image ally2Health;
    public Image ally3Health;
    public Image ally4Health;


    //ability info
    public Text ability1;
    public Text ability2;
    public Text ability3;
    public Text ability4;
    public Text ability5;

    public static readonly string STAT_PREFS_ENFORCER_LEVEL = "stats_prefs_level_1";
    public static readonly string STAT_PREFS_MEDIC_LEVEL = "stats_prefs_level_2";
    public static readonly string STAT_PREFS_RIFLEMAN_LEVEL = "stats_prefs_level_3";
    public static readonly string STAT_PREFS_ENGINEER_LEVEL = "stats_prefs_level_4";

    public static readonly string STAT_PREFS_ENFORCER_RANK = "stats_prefs_level_1";
    public static readonly string STAT_PREFS_MEDIC_RANK = "stats_prefs_level_2";
    public static readonly string STAT_PREFS_RIFLEMAN_RANK = "stats_prefs_level_3";
    public static readonly string STAT_PREFS_ENGINEER_RANK = "stats_prefs_level_4";


    // Use this for initialization
    void Start()
    {

        //init units
        enforcer = new Enforcer();
        medic = new Medic();
        rifleman = new Rifleman();
        engineer = new Engineer();

        //set stats if we leveled up earlier (health setting will be gone in update)
        enforcer.SetStats(PlayerPrefs.GetInt(STAT_PREFS_ENFORCER_LEVEL, 0), PlayerPrefs.GetInt(STAT_PREFS_ENFORCER_RANK, 0));
        medic.SetStats(PlayerPrefs.GetInt(STAT_PREFS_MEDIC_LEVEL, 0), PlayerPrefs.GetInt(STAT_PREFS_MEDIC_RANK, 1));
        rifleman.SetStats(PlayerPrefs.GetInt(STAT_PREFS_RIFLEMAN_LEVEL, 0), PlayerPrefs.GetInt(STAT_PREFS_RIFLEMAN_RANK, 2));
        engineer.SetStats(PlayerPrefs.GetInt(STAT_PREFS_RIFLEMAN_LEVEL, 0), PlayerPrefs.GetInt(STAT_PREFS_RIFLEMAN_RANK, 3));

        freightEnemy1 = new Freight();
        freightEnemy1.SetStats(1, 0);
        freightEnemy2 = new Freight();
        freightEnemy2.SetStats(1, 1);
        freightEnemy3 = new Freight();
        freightEnemy3.SetStats(1, 2);
        freightEnemy4 = new Freight();
        freightEnemy4.SetStats(1, 3);

        allies = new Unit[] { enforcer, medic, rifleman, engineer };
        enemies = new Unit[] { freightEnemy1, freightEnemy2, freightEnemy3, freightEnemy4 };

        //hardcode setup
		order = new Unit[] { enforcer, medic, rifleman, engineer, freightEnemy1, freightEnemy2, freightEnemy3, freightEnemy4 };
        //order = enforcer.Order(allies, enemies);

        //We are going to resort our allies and enemies array based off order chosen now
        int friendlyIndex = 0;
        int enemyIndex = 0;

        //for (int i = 0; i < order.Length; i++)
        //{
        //    if (order[i].GetFriendly() == true)
        //    {
        //        allies[friendlyIndex] = order[i];
        //        ++friendlyIndex;
        //    }
        //    else
        //    {
        //        enemies[enemyIndex] = order[i];
        //        ++enemyIndex;
        //    }
        //}

        for (int y = 0; y < order.Length; y++)
        {
            Debug.Log("order: " + order[y].GetType().ToString());
        }

        for (int y = 0; y < allies.Length; y++)
        {
            Debug.Log("allies: " + allies[y].GetType().ToString());
        }

        for (int y = 0; y < enemies.Length; y++)
        {
            Debug.Log("enemies: " + enemies[y].GetType().ToString());
        }

        //enforcer to start
        indexOfOrder = 0;
        currentCharacter = order[indexOfOrder];


        //popUP.SetActive(false);
        //popUPAttack.SetActive(false);
        hidePopUp();
        hidePopUpAttack();
        hideStepEnemyAttackPopUp();
        hidePopUpVictory();
        hidePopUpFailure();
        hidePopUpHit();
        hidePopUpMiss();
        enableAbilityButtons();
    }

    // Update is called once per frame
    void Update()
    {
        //load main menu if escape is pressed.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel(0);
        }


        //these for loops below are bad coding practice and slow things down but we are lazy so they are here...

        //check if all allies are dead
        int allyTally = 0;
        for (int i = 0; i < allies.Length; i++)
        {
            if (allies[i].GetHealth() <= 0)
            {
                ++allyTally;
                if (allyTally == allies.Length)
                {
                    showPopUpFailure(); //if they are all dead show fail
                }
            }
        }

        //check if all enemies are dead
        int enemyTally = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetHealth() <= 0)
            {
                ++enemyTally;
                if (enemyTally == enemies.Length)
                {
                    showPopUpVictory(); //if they are all dead show win

					//level up, store in player prefs
					PlayerPrefs.SetInt(STAT_PREFS_ENFORCER_LEVEL, PlayerPrefs.GetInt(STAT_PREFS_ENFORCER_LEVEL, 1) + 1);
					PlayerPrefs.SetInt(STAT_PREFS_MEDIC_LEVEL, PlayerPrefs.GetInt(STAT_PREFS_MEDIC_LEVEL, 1) + 1);
					PlayerPrefs.SetInt(STAT_PREFS_RIFLEMAN_LEVEL, PlayerPrefs.GetInt(STAT_PREFS_RIFLEMAN_LEVEL, 1) + 1);
					PlayerPrefs.SetInt(STAT_PREFS_ENGINEER_LEVEL, PlayerPrefs.GetInt(STAT_PREFS_ENGINEER_LEVEL, 1) + 1);
                }
            }
        }

        for (int i = 0; i < order.Length; i++)
        {
            if (order[i].GetHealth() <= 0)
            {
                characters[i].SetActive(false);
            }
        }

        if (order[indexOfOrder].GetHealth() > 0)
        {
            currentCharacter = order[indexOfOrder];
        }
        else
        {
            if (indexOfOrder < order.Length - 1)
            {
                ++indexOfOrder;
                currentCharacter = order[indexOfOrder];
            }
            else
            {
                indexOfOrder = 0;
                currentCharacter = order[indexOfOrder];
            }
        }
        configAbilityRanges(); //every button click load up new ability array for attackingUnit[] TempArr = new Unit[allies.Length];

        currentCharacterArrow.transform.position = new Vector3(characters[indexOfOrder].transform.position.x, characters[indexOfOrder].transform.position.y + 2.5f, characters[indexOfOrder].transform.position.z);

        currentCharacterText.text = "Current Character: " + currentCharacter.GetType().ToString();

        float scale = ((float)allies[0].GetHealth() / (float)allies[0].GetBaseHealth());
        ally1Health.rectTransform.localScale = new Vector3(scale, 1f, 1f);
        scale = ((float)allies[1].GetHealth() / (float)allies[1].GetBaseHealth());
        ally2Health.rectTransform.localScale = new Vector3(scale, 1f, 1f);
        scale = ((float)allies[2].GetHealth() / (float)allies[2].GetBaseHealth());
        ally3Health.rectTransform.localScale = new Vector3(scale, 1f, 1f);
        scale = ((float)allies[3].GetHealth() / (float)allies[3].GetBaseHealth());
        ally4Health.rectTransform.localScale = new Vector3(scale, 1f, 1f);

        scale = ((float)enemies[0].GetHealth() / (float)enemies[0].GetBaseHealth());
        enemy1Health.rectTransform.localScale = new Vector3(scale, 1f, 1f);
        scale = ((float)enemies[1].GetHealth() / (float)enemies[1].GetBaseHealth());
        enemy2Health.rectTransform.localScale = new Vector3(scale, 1f, 1f);
        scale = ((float)enemies[2].GetHealth() / (float)enemies[2].GetBaseHealth());
        enemy3Health.rectTransform.localScale = new Vector3(scale, 1f, 1f);
        scale = ((float)enemies[3].GetHealth() / (float)enemies[3].GetBaseHealth());
        enemy4Health.rectTransform.localScale = new Vector3(scale, 1f, 1f);

        //get attack ranges
        if (currentCharacter.GetFriendly() == true)
        {
            if (currentCharacter.GetCategory() == "Enforcer")
            {
                abilityButtons[0].image.sprite = enforcerAbilityImages[0];
                abilityButtons[1].image.sprite = enforcerAbilityImages[1];
                abilityButtons[2].image.sprite = enforcerAbilityImages[2];
                abilityButtons[3].image.sprite = enforcerAbilityImages[3];
                abilityButtons[4].image.sprite = enforcerAbilityImages[4];
            }
            if (currentCharacter.GetCategory() == "Medic")
            {
                abilityButtons[0].image.sprite = medicAbilityImages[0];
                abilityButtons[1].image.sprite = medicAbilityImages[1];
                abilityButtons[2].image.sprite = medicAbilityImages[2];
                abilityButtons[3].image.sprite = medicAbilityImages[3];
                abilityButtons[4].image.sprite = medicAbilityImages[4];
            }
            if (currentCharacter.GetCategory() == "Engineer")
            {
                abilityButtons[0].image.sprite = engineerAbilityImages[0];
                abilityButtons[1].image.sprite = engineerAbilityImages[1];
                abilityButtons[2].image.sprite = engineerAbilityImages[2];
                abilityButtons[3].image.sprite = engineerAbilityImages[3];
                abilityButtons[4].image.sprite = engineerAbilityImages[4];
            }
            if (currentCharacter.GetCategory() == "Rifleman")
            {
                abilityButtons[0].image.sprite = riflemanAbilityImages[0];
                abilityButtons[1].image.sprite = riflemanAbilityImages[1];
                abilityButtons[2].image.sprite = riflemanAbilityImages[2];
                abilityButtons[3].image.sprite = riflemanAbilityImages[3];
                abilityButtons[4].image.sprite = riflemanAbilityImages[4];
            }

            ability1EnableArray = currentCharacter.GetAttackRange(0);
            ability2EnableArray = currentCharacter.GetAttackRange(1);
            ability3EnableArray = currentCharacter.GetAttackRange(2);
            ability4EnableArray = currentCharacter.GetAttackRange(3);
            ability5EnableArray = currentCharacter.GetAttackRange(4);
        }

        if (currentCharacter.GetFriendly() == false)
        {

            showStepEnemyAttackPopUp(); //so we can step through enemy attacks
            disableAbilityButtons(); //don't want to interact with buttons if enemy is going
            hidePopUp();
        }
    }

    public void buttonClicked()
    {
        //new turn so hide these until needed again
        hidePopUpHit();
        hidePopUpMiss();

        currentButtonClicked = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(currentButtonClicked);
        switch (currentButtonClicked)
        {
            case "levelSelect":
                Application.LoadLevel(1);
                break;
            case "retry":
                Application.LoadLevel(Application.loadedLevel);
                break;
            //clicking abilities
            case "ability1":
                showPopUpAttack();
                hidePopUp();
                currentAbility = 0;
                break;
            case "ability2":
                showPopUpAttack();
                hidePopUp();
                currentAbility = 1;
                break;
            case "ability3":
                showPopUpAttack();
                hidePopUp();
                currentAbility = 2;
                break;
            case "ability4":
                showPopUpAttack();
                hidePopUp();
                currentAbility = 3;
                break;
            case "ability5":
                showPopUpAttack();
                hidePopUp();
                currentAbility = 4;
                break;
            //clicking to attack characters
            case "Enemy1":
                hidePopUpAttack();
                hidePopUp();
                if (currentCharacter.GetFriendly() == true && currentCharacter.GetHealth() > 0)
                {
                    if (currentCharacter.MakeMove(currentAbility, allies, enemies, enemies[0]) == true)
                    {
                        shoot.Play();
                        showPopUpHit();
                    }
                    else
                    {
                        showPopUpMiss();
                    }
                    ++indexOfOrder;
                }
                break;
            case "Enemy2":
                hidePopUpAttack();
                hidePopUp();
                if (currentCharacter.GetFriendly() == true && currentCharacter.GetHealth() > 0)
                {
                    if (currentCharacter.MakeMove(currentAbility, allies, enemies, enemies[1]) == true)
                    {
                        showPopUpHit();
                        shoot.Play();
                    }
                    else
                    {
                        showPopUpMiss();
                    }
                    ++indexOfOrder;
                }
                break;
            case "Enemy3":
                hidePopUpAttack();
                hidePopUp();
                if (currentCharacter.GetFriendly() == true && currentCharacter.GetHealth() > 0)
                {
                    if (currentCharacter.MakeMove(currentAbility, allies, enemies, enemies[2]) == true)
                    {
                        showPopUpHit();
                        shoot.Play();
                    }
                    else
                    {
                        showPopUpMiss();
                    }
                    ++indexOfOrder;
                }
                break;
            case "Enemy4":
                hidePopUpAttack();
                hidePopUp();
                if (currentCharacter.GetFriendly() == true && currentCharacter.GetHealth() > 0)
                {
                    if (currentCharacter.MakeMove(currentAbility, allies, enemies, enemies[3]) == true)
                    {
                        showPopUpHit();
                        shoot.Play();
                    }
                    else
                    {
                        showPopUpMiss();
                    }
                    ++indexOfOrder;
                }
                break;
            case "selfHeal":
                hidePopUpAttack();
                hidePopUp();
                if (currentCharacter.GetFriendly() == true && currentCharacter.GetHealth() > 0)
                {
                    if (currentCharacter.MakeMove(currentAbility, allies, enemies, order[indexOfOrder]) == true)
                    {
                        showPopUpHit();
                        heal.Play();

                    }
                    else
                    {
                        showPopUpMiss();
                    }
                    ++indexOfOrder;
                }
                break;
            case "healOther":
                hidePopUpAttack();
                hidePopUp();
                if (currentCharacter.GetFriendly() == true && currentCharacter.GetHealth() > 0)
                {
                    if (currentCharacter.MakeMove(currentAbility, allies, enemies, allies[0]) == true) //make this 0 for now
                    {
                        showPopUpHit();
                        heal.Play();
                    }
                    else
                    {
                        showPopUpMiss();
                    }
                    ++indexOfOrder;
                }
                break;
            case "allEnemy":
                hidePopUpAttack();
                hidePopUp();
                if (currentCharacter.GetFriendly() == true && currentCharacter.GetHealth() > 0)
                {
                    if (currentCharacter.MakeMove(currentAbility, allies, enemies, enemies[0]) == true || currentCharacter.MakeMove(currentAbility, allies, enemies, enemies[1]) == true
                        || currentCharacter.MakeMove(currentAbility, allies, enemies, enemies[2]) == true || currentCharacter.MakeMove(currentAbility, allies, enemies, enemies[3]) == true) //make this 0 for now
                    {
                        showPopUpHit();
                    }
                    else
                    {
                        showPopUpMiss();
                    }
                    ++indexOfOrder;
                }
                break;
            case "StepAttackEnemy":
                //make a simple ai move
                int move = Random.Range(0, 2);
                int allyHit = Random.Range(0, 4);
                //if player isn't dead
                if (currentCharacter.GetHealth() > 0)
                {
                    currentCharacter.MakeMove(move, allies, enemies, allies[allyHit]);
                }
                if (indexOfOrder < order.Length - 1)
                {
                    ++indexOfOrder;
                }
                else
                {
                    indexOfOrder = 0;
                }
                hideStepEnemyAttackPopUp();
                enableAbilityButtons();
                break;
        }
    }

    private void configAbilityRanges()
    {
        //config what buttons are enabled based off what ability selected:
        switch (currentButtonClicked)
        {
            case "ability1":
                for (int i = 0; i < 7; i++)
                {
                    if (ability1EnableArray[i] == true)
                    {
                        enemyAttackButtons[i].interactable = true;
                        if (i == 4)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageAlly; //self heal
                        }
                        else if (i == 5)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageHealAlly; //heal other
                        }
                        else
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageEnemy; //enemy
                        }
                    }
                    else
                    {
                        enemyAttackButtons[i].interactable = false;
                        enemyAttackButtons[i].image.sprite = disabledButtonImage;
                    }
                }
                break;
            case "ability2":
                for (int i = 0; i < 7; i++)
                {
                    if (ability2EnableArray[i] == true)
                    {
                        enemyAttackButtons[i].interactable = true;
                        if (i == 4)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageAlly; //self heal
                        }
                        else if (i == 5)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageHealAlly; //heal other
                        }
                        else
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageEnemy; //enemy
                        }
                    }
                    else
                    {
                        enemyAttackButtons[i].interactable = false;
                        enemyAttackButtons[i].image.sprite = disabledButtonImage;
                    }
                }
                break;
            case "ability3":
                for (int i = 0; i < 7; i++)
                {
                    if (ability3EnableArray[i] == true)
                    {
                        enemyAttackButtons[i].interactable = true;
                        if (i == 4)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageAlly; //self heal
                        }
                        else if (i == 5)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageHealAlly; //heal other
                        }
                        else
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageEnemy; //enemy
                        }
                    }
                    else
                    {
                        enemyAttackButtons[i].interactable = false;
                        enemyAttackButtons[i].image.sprite = disabledButtonImage;
                    }
                }
                break;
            case "ability4":
                for (int i = 0; i < 7; i++)
                {
                    if (ability4EnableArray[i] == true)
                    {
                        enemyAttackButtons[i].interactable = true;
                        if (i == 4)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageAlly; //self heal
                        }
                        else if (i == 5)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageHealAlly; //heal other
                        }
                        else
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageEnemy; //enemy
                        }
                    }
                    else
                    {
                        enemyAttackButtons[i].interactable = false;
                        enemyAttackButtons[i].image.sprite = disabledButtonImage;
                    }
                }
                break;
            case "ability5":
                for (int i = 0; i < 7; i++)
                {
                    if (ability5EnableArray[i] == true)
                    {
                        enemyAttackButtons[i].interactable = true;
                        if (i == 4)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageAlly; //self heal
                        }
                        else if (i == 5)
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageHealAlly; //heal other
                        }
                        else
                        {
                            enemyAttackButtons[i].image.sprite = enabledButtonImageEnemy; //enemy
                        }
                    }
                    else
                    {
                        enemyAttackButtons[i].interactable = false;
                        enemyAttackButtons[i].image.sprite = disabledButtonImage;
                    }
                }
                break;
        }
    }

    public void showPopUpAttack()
    {
        //always move popUpAttack when clicked
        popUPAttack.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);

        //show attack popup
        if (!popUPAttackEnabled)
        {
            popUPAttack.SetActive(true);
            popUPAttackEnabled = true;
        }

    }

    public void hidePopUpAttack()
    {
        popUPAttack.SetActive(false);
        popUPAttackEnabled = false;
    }

    public void showPopUp(int ability)
    {
        if (ability == 0)
        {
            if (currentCharacter.GetType().ToString() == "Enforcer")
            {
                ability1.text = "HEAVY SWING" + "\n" + "Crit: " + enforcer.CritMods[0].ToString() + "%\n" + "Damage: " + (100 + enforcer.DmgMods[0] * 100).ToString() + "%\n" + "Accuracy: " + enforcer.AccMods[0].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Medic")
            {
                ability1.text = "ADRENALINE RUSH" + "\n" + "(Increases damage, dodge)" + "\n" + "Crit: " + medic.CritMods[0].ToString() + "%\n" + "Damage: " + (100 + medic.DmgMods[0] * 100).ToString() + "%\n" + "Accuracy: " + medic.AccMods[0].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Engineer")
            {
                ability1.text = "FLASH BANG" + "\n" + "Stuns" + "\n" + "Crit: " + engineer.CritMods[0].ToString() + "%\n" + "Damage: " + (100 + engineer.DmgMods[0] * 100).ToString() + "%\n" + "Accuracy: " + engineer.AccMods[0].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Rifleman")
            {
                ability1.text = "BAYONET STAB" + "\n" + "Crit: " + rifleman.CritMods[0].ToString() + "%\n" + "Damage: " + (100 + rifleman.DmgMods[0] * 100).ToString() + "%\n" + "Accuracy: " + rifleman.AccMods[0].ToString();
            }
        }

        if (ability == 1)
        {
            if (currentCharacter.GetType().ToString() == "Enforcer")
            {
                ability2.text = "KICK" + "\n" + "(Knocks back target)" + "\n" + "Crit: " + enforcer.CritMods[1].ToString() + "%\n" + "Damage: " + (100 + enforcer.DmgMods[1] * 100).ToString() + "%\n" + "Accuracy: " + enforcer.AccMods[1].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Medic")
            {
                ability2.text = "BULWARK" + "\n" + "(Increases armor, decreases dodge)" + "\n" + "Crit: " + medic.CritMods[1].ToString() + "%\n" + "Damage: " + (100 + medic.DmgMods[1] * 100).ToString() + "%\n" + "Accuracy: " + medic.AccMods[1].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Engineer")
            {
                ability2.text = "ION PULSE" + "\n" + "(Stuns mechanical enemies, slows everyone else)" + "\n" + "Crit: " + engineer.CritMods[1].ToString() + "%\n" + "Damage: " + (100 + engineer.DmgMods[1] * 100).ToString() + "%\n" + "Accuracy: " + engineer.AccMods[1].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Rifleman")
            {
                ability2.text = "NET" + "\n" + "(Debuffs enemies speed+accuracy)" + "\n" + "Crit: " + rifleman.CritMods[1].ToString() + "%\n" + "Damage: " + (100 + rifleman.DmgMods[1] * 100).ToString() + "%\n" + "Accuracy: " + rifleman.AccMods[1].ToString();
            }
        }


        if (ability == 2)
        {
            if (currentCharacter.GetType().ToString() == "Enforcer")
            {
                ability3.text = "SLICE" + "\n" + "(Inflicts bleed)" + "\n" + "Crit: " + enforcer.CritMods[2].ToString() + "%\n" + "Damage: " + (100 + enforcer.DmgMods[2] * 100).ToString() + "%\n" + "Accuracy: " + enforcer.AccMods[2].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Medic")
            {
                ability3.text = "HEALING WAVE" + "\n" + "Crit: " + medic.CritMods[2].ToString() + "%\n" + "Damage: " + (100 + medic.DmgMods[2] * 100).ToString() + "%\n" + "Accuracy: " + medic.AccMods[2].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Engineer")
            {
                ability3.text = "LIGHT WALL" + "\n" + "(Block 1 projectile)" + "\n" + "Crit: " + engineer.CritMods[2].ToString() + "%\n" + "Damage: " + (100 + engineer.DmgMods[2] * 100).ToString() + "%\n" + "Accuracy: " + engineer.AccMods[2].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Rifleman")
            {
                ability3.text = "RELOAD" + "\n" + "(Self heal, amplify speed and damage)" + "\n" + "Crit: " + rifleman.CritMods[2].ToString() + "%\n" + "Damage: " + (100 + rifleman.DmgMods[2] * 100).ToString() + "%\n" + "Accuracy: " + rifleman.AccMods[2].ToString();
            }
        }

        if (ability == 3)
        {
            if (currentCharacter.GetType().ToString() == "Enforcer")
            {
                ability4.text = "STEROIDS" + "\n" + "(Self heal, plus speed boost)" + "\n" + "Crit: " + enforcer.CritMods[3].ToString() + "%\n" + "Damage: " + (100 + enforcer.DmgMods[3] * 100).ToString() + "%\n" + "Accuracy: " + enforcer.AccMods[3].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Medic")
            {
                ability4.text = "PISTOL SHOT" + "\n" + "Crit: " + medic.CritMods[3].ToString() + "%\n" + "Damage: " + (100 + medic.DmgMods[3] * 100).ToString() + "%\n" + "Accuracy: " + medic.AccMods[3].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Engineer")
            {
                ability4.text = "RATCHET GUN" + "\n" + "(Increases damage)" + "\n" + "Crit: " + engineer.CritMods[3].ToString() + "%\n" + "Damage: " + (100 + engineer.DmgMods[3] * 100).ToString() + "%\n" + "Accuracy: " + engineer.AccMods[3].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Rifleman")
            {
                ability4.text = "RIFLE SHOT" + "\n" + "Crit: " + rifleman.CritMods[3].ToString() + "%\n" + "Damage: " + (100 + rifleman.DmgMods[3] * 100).ToString() + "%\n" + "Accuracy: " + rifleman.AccMods[3].ToString();
            }
        }


        if (ability == 4)
        {
            if (currentCharacter.GetType().ToString() == "Enforcer")
            {
                ability5.text = "WAR CHANT" + "\n" + "Crit: " + enforcer.CritMods[4].ToString() + "%\n" + "Damage: " + (100 + enforcer.DmgMods[4] * 100).ToString() + "%\n" + "Accuracy: " + enforcer.AccMods[4].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Medic")
            {
                ability5.text = "TASER" + "\n" + "(Stuns, decreases speed)" + "\n" + "Crit: " + medic.CritMods[4].ToString() + "%\n" + "Damage: " + (100 + medic.DmgMods[4] * 100).ToString() + "%\n" + "Accuracy: " + medic.AccMods[4].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Engineer")
            {
                ability5.text = "SNARE" + "\n" + "(Brings the enemy closer)" + "\n" + "Crit: " + engineer.CritMods[4].ToString() + "%\n" + "Damage: " + (100 + engineer.DmgMods[4] * 100).ToString() + "%\n" + "Accuracy: " + engineer.AccMods[4].ToString();
            }
            if (currentCharacter.GetType().ToString() == "Rifleman")
            {
                ability5.text = "SHOTGUN" + "\n" + "(Causes knockback to target)" + "\n" + "Crit: " + rifleman.CritMods[4].ToString() + "%\n" + "Damage: " + (100 + rifleman.DmgMods[4] * 100).ToString() + "%\n" + "Accuracy: " + rifleman.AccMods[4].ToString();
            }
        }

        if (!popUPEnabled)
        {
            popUP.SetActive(true);
            popUP.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
            popUPEnabled = true;
        }
    }

    public void hidePopUp()
    {
        popUP.SetActive(false);
        popUPEnabled = false;
    }

    public void hideStepEnemyAttackPopUp()
    {
        stepEnemyAttackPopUp.SetActive(false);
    }

    public void showStepEnemyAttackPopUp()
    {
        stepEnemyAttackPopUp.SetActive(true);
    }

    public void hidePopUpVictory()
    {
        popUpVictory.SetActive(false);
    }

    public void showPopUpVictory()
    {
        popUpVictory.SetActive(true);
    }

    public void hidePopUpFailure()
    {
        popUpFailure.SetActive(false);
    }

    public void showPopUpFailure()
    {
        popUpFailure.SetActive(true);
    }

    public void hidePopUpHit()
    {
        popUpHit.SetActive(false);
    }

    public void showPopUpHit()
    {
        popUpHit.SetActive(true);
    }

    public void hidePopUpMiss()
    {
        popUpMiss.SetActive(false);
    }

    public void showPopUpMiss()
    {
        popUpMiss.SetActive(true);
    }

    public void disableAbilityButtons()
    {
        for (int i = 0; i < abilityButtons.Length; i++)
            abilityButtons[i].interactable = false;
    }

    public void enableAbilityButtons()
    {
        for (int i = 0; i < abilityButtons.Length; i++)
            abilityButtons[i].interactable = true;
    }
}
