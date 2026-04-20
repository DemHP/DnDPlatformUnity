using System;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Windows.Forms;
using Application = UnityEngine.Application;
using UnityEngine.EventSystems;

[Serializable]
public class CharacterData
{
    // Strings
    public string charName, classes, background, race, alignment;
    public string characterPortraitLocation;

    // Basic stats
    public int level, profBonus, inspiration, armorClass, initiative, speed, currentHp, tempHp;

    // Ability scores and skills
    public int str, athletics;
    public int dex, acrobatics, slightOfHand, stealth;
    public int con;
    public int intelligence, arcana, history, invest, nature, religion;
    public int wis, animalHandling, insight, medicine, perception, survival;
    public int charisma, deception, intimidation, performance, persuasion;

    // Saving throws
    public bool strSavingThrow, dexSavingThrow, conSavingThrow,
                intSavingThrow, wisSavingThrow, charismaSavingThrow;

    // Skill checks toggles
    public bool acrobaticsCheck, slightOfHandCheck, stealthCheck,
                arcanaCheck, historyCheck, investCheck, natureCheck,
                religionCheck, animalCheck, insightCheck, medicineCheck,
                percepCheck, survivalCheck, deceptionCheck, intimCheck,
                performanceCheck, persuassionCheck;
}

public class CharacterSaveManager : MonoBehaviour
{
    private string CharactersFolder => Path.Combine(Application.persistentDataPath, "Characters");
    private string _pendingCharacterPath;
    public TMP_InputField saveNameInputField;


    [Header("Character Portrait Location")]
    public string characterPortraitLocation;
    public Image characterPortraitImage;

    [Header("Basic Info")]
    public TMP_InputField charNameInput;
    public TMP_InputField classesInput;
    public TMP_InputField backgroundInput;
    public TMP_InputField raceInput;
    public TMP_InputField alignmentInput;

    [Header("Stats")]
    public TMP_InputField levelInput;
    public TMP_InputField profBonusInput;
    public TMP_InputField inspirationInput;
    public TMP_InputField armorClassInput;
    public TMP_InputField initiativeInput;
    public TMP_InputField speedInput;
    public TMP_InputField currentHpInput;
    public TMP_InputField tempHpInput;

    [Header("Abilities and Skills")]
    public TMP_InputField strInput;
    public TMP_InputField athleticsInput;

    public TMP_InputField dexInput;
    public TMP_InputField acrobaticsInput;
    public TMP_InputField slightOfHandInput;
    public TMP_InputField stealthInput;

    public TMP_InputField conInput; // FINISH THIS AIDAN

    public TMP_InputField intelligenceInput;
    public TMP_InputField arcanaInput;
    public TMP_InputField historyInput;
    public TMP_InputField investInput;
    public TMP_InputField natureInput;
    public TMP_InputField religionInput;

    public TMP_InputField wisInput;
    public TMP_InputField animalHandlingInput;
    public TMP_InputField insightInput;
    public TMP_InputField medicineInput;
    public TMP_InputField perceptionInput;
    public TMP_InputField survivalInput;

    public TMP_InputField charismaInput;
    public TMP_InputField deceptionInput;
    public TMP_InputField intimidationInput;
    public TMP_InputField performanceInput;
    public TMP_InputField persuasionInput;

    [Header("Saving Throws")]
    public Toggle strSavingThrowToggle;
    public Toggle dexSavingThrowToggle;
    public Toggle conSavingThrowToggle;
    public Toggle intSavingThrowToggle;
    public Toggle wisSavingThrowToggle;
    public Toggle charismaSavingThrowToggle;

    [Header("Skill Checks")]
    public Toggle acrobaticsCheckToggle;
    public Toggle slightOfHandCheckToggle;
    public Toggle stealthCheckToggle;

    public Toggle arcanaCheckToggle;
    public Toggle historyCheckToggle;
    public Toggle investCheckToggle;
    public Toggle natureCheckToggle;
    public Toggle religionCheckToggle;

    public Toggle animalCheckToggle;
    public Toggle insightCheckToggle;
    public Toggle medicineCheckToggle;
    public Toggle percepCheckToggle;
    public Toggle survivalCheckToggle;

    public Toggle deceptionCheckToggle;
    public Toggle intimCheckToggle;
    public Toggle performanceCheckToggle;
    public Toggle persuassionCheckToggle;


    [Header("Current Character Loaded")]

    public CharacterData currCharacter = new CharacterData();

    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "character.json");
    }

    public void SaveCharacter(string characterFileName)
    {
        if (!Directory.Exists(CharactersFolder))
        {
            Directory.CreateDirectory(CharactersFolder);
        }
        UpdateCharacterDataFromUI();

        characterFileName = saveNameInputField.text;

        // Ensure it has .json extension
        if (!characterFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            characterFileName += ".json";
        }

        string json = JsonUtility.ToJson(currCharacter, false); // false = compact JSON
        string fullPath = Path.Combine(CharactersFolder, characterFileName);
        try
        {
            File.WriteAllText(fullPath, json);
            Debug.Log($"Character saved to: {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save character: " + e.Message);
        }
    }

    private void UpdateCharacterDataFromUI()
    {
        // --- Basic Info ---
        currCharacter.charName = charNameInput.text;
        currCharacter.classes = classesInput.text;
        currCharacter.background = backgroundInput.text;
        currCharacter.race = raceInput.text;
        currCharacter.alignment = alignmentInput.text;

        currCharacter.characterPortraitLocation = characterPortraitLocation;

        // --- Stats ---
        currCharacter.level = int.TryParse(levelInput.text, out int lvl) ? lvl : 0;
        currCharacter.profBonus = int.TryParse(profBonusInput.text, out int pb) ? pb : 0;
        currCharacter.inspiration = int.TryParse(inspirationInput.text, out int insp) ? insp : 0;
        currCharacter.armorClass = int.TryParse(armorClassInput.text, out int ac) ? ac : 0;
        currCharacter.initiative = int.TryParse(initiativeInput.text, out int init) ? init : 0;
        currCharacter.speed = int.TryParse(speedInput.text, out int spd) ? spd : 0;
        currCharacter.currentHp = int.TryParse(currentHpInput.text, out int chp) ? chp : 0;
        currCharacter.tempHp = int.TryParse(tempHpInput.text, out int thp) ? thp : 0;

        // --- Ability Scores & Skills ---
        currCharacter.str = int.TryParse(strInput.text, out int s) ? s : 0;
        currCharacter.athletics = int.TryParse(athleticsInput.text, out int ath) ? ath : 0;

        currCharacter.dex = int.TryParse(dexInput.text, out int d) ? d : 0;
        currCharacter.acrobatics = int.TryParse(acrobaticsInput.text, out int a) ? a : 0;
        currCharacter.slightOfHand = int.TryParse(slightOfHandInput.text, out int sh) ? sh : 0;
        currCharacter.stealth = int.TryParse(stealthInput.text, out int st) ? st : 0;

        currCharacter.con = int.TryParse(conInput.text, out int con) ? con : 0;

        currCharacter.intelligence = int.TryParse(intelligenceInput.text, out int i) ? i : 0;
        currCharacter.arcana = int.TryParse(arcanaInput.text, out int ar) ? ar : 0;
        currCharacter.history = int.TryParse(historyInput.text, out int hi) ? hi : 0;
        currCharacter.invest = int.TryParse(investInput.text, out int inv) ? inv : 0;
        currCharacter.nature = int.TryParse(natureInput.text, out int nat) ? nat : 0;
        currCharacter.religion = int.TryParse(religionInput.text, out int rel) ? rel : 0;

        currCharacter.wis = int.TryParse(wisInput.text, out int w) ? w : 0;
        currCharacter.animalHandling = int.TryParse(animalHandlingInput.text, out int ah) ? ah : 0;
        currCharacter.insight = int.TryParse(insightInput.text, out int ins) ? ins : 0;
        currCharacter.medicine = int.TryParse(medicineInput.text, out int med) ? med : 0;
        currCharacter.perception = int.TryParse(perceptionInput.text, out int perc) ? perc : 0;
        currCharacter.survival = int.TryParse(survivalInput.text, out int surv) ? surv : 0;

        currCharacter.charisma = int.TryParse(charismaInput.text, out int ch) ? ch : 0;
        currCharacter.deception = int.TryParse(deceptionInput.text, out int dec) ? dec : 0;
        currCharacter.intimidation = int.TryParse(intimidationInput.text, out int inti) ? inti : 0;
        currCharacter.performance = int.TryParse(performanceInput.text, out int perf) ? perf : 0;
        currCharacter.persuasion = int.TryParse(persuasionInput.text, out int pers) ? pers : 0;

        // --- Saving Throws ---
        currCharacter.strSavingThrow = strSavingThrowToggle.isOn;
        currCharacter.dexSavingThrow = dexSavingThrowToggle.isOn;
        currCharacter.conSavingThrow = conSavingThrowToggle.isOn;
        currCharacter.intSavingThrow = intSavingThrowToggle.isOn;
        currCharacter.wisSavingThrow = wisSavingThrowToggle.isOn;
        currCharacter.charismaSavingThrow = charismaSavingThrowToggle.isOn;

        // --- Skill Checks ---
        currCharacter.acrobaticsCheck = acrobaticsCheckToggle.isOn;
        currCharacter.slightOfHandCheck = slightOfHandCheckToggle.isOn;
        currCharacter.stealthCheck = stealthCheckToggle.isOn;

        currCharacter.arcanaCheck = arcanaCheckToggle.isOn;
        currCharacter.historyCheck = historyCheckToggle.isOn;
        currCharacter.investCheck = investCheckToggle.isOn;
        currCharacter.natureCheck = natureCheckToggle.isOn;
        currCharacter.religionCheck = religionCheckToggle.isOn;

        currCharacter.animalCheck = animalCheckToggle.isOn;
        currCharacter.insightCheck = insightCheckToggle.isOn;
        currCharacter.medicineCheck = medicineCheckToggle.isOn;
        currCharacter.percepCheck = percepCheckToggle.isOn;
        currCharacter.survivalCheck = survivalCheckToggle.isOn;

        currCharacter.deceptionCheck = deceptionCheckToggle.isOn;
        currCharacter.intimCheck = intimCheckToggle.isOn;
        currCharacter.performanceCheck = performanceCheckToggle.isOn;
        currCharacter.persuassionCheck = persuassionCheckToggle.isOn;
    }

    private void UpdateUIFromCharacterData()
    {
        // Basic Info
        charNameInput.text = currCharacter.charName;
        classesInput.text = currCharacter.classes;
        backgroundInput.text = currCharacter.background;
        raceInput.text = currCharacter.race;
        alignmentInput.text = currCharacter.alignment;

        // Update Character Portrait
        Sprite portrait = LoadSpriteFromPath(currCharacter.characterPortraitLocation);

        characterPortraitImage.sprite = portrait;

        // hide if missing
        characterPortraitImage.enabled = portrait != null;


        // Stats
        levelInput.text = currCharacter.level.ToString();
        profBonusInput.text = currCharacter.profBonus.ToString();
        inspirationInput.text = currCharacter.inspiration.ToString();
        armorClassInput.text = currCharacter.armorClass.ToString();
        initiativeInput.text = currCharacter.initiative.ToString();
        speedInput.text = currCharacter.speed.ToString();
        currentHpInput.text = currCharacter.currentHp.ToString();
        tempHpInput.text = currCharacter.tempHp.ToString();

        // Ability scores and skills
        strInput.text = currCharacter.str.ToString();
        athleticsInput.text = currCharacter.athletics.ToString();

        dexInput.text = currCharacter.dex.ToString();
        acrobaticsInput.text = currCharacter.acrobatics.ToString();
        slightOfHandInput.text = currCharacter.slightOfHand.ToString();
        stealthInput.text = currCharacter.stealth.ToString();

        conInput.text = currCharacter.con.ToString();

        intelligenceInput.text = currCharacter.intelligence.ToString();
        arcanaInput.text = currCharacter.arcana.ToString();
        historyInput.text = currCharacter.history.ToString();
        investInput.text = currCharacter.invest.ToString();
        natureInput.text = currCharacter.nature.ToString();
        religionInput.text = currCharacter.religion.ToString();

        wisInput.text = currCharacter.wis.ToString();
        animalHandlingInput.text = currCharacter.animalHandling.ToString();
        insightInput.text = currCharacter.insight.ToString();
        medicineInput.text = currCharacter.medicine.ToString();
        perceptionInput.text = currCharacter.perception.ToString();
        survivalInput.text = currCharacter.survival.ToString();

        charismaInput.text = currCharacter.charisma.ToString();
        deceptionInput.text = currCharacter.deception.ToString();
        intimidationInput.text = currCharacter.intimidation.ToString();
        performanceInput.text = currCharacter.performance.ToString();
        persuasionInput.text = currCharacter.persuasion.ToString();

        // Saving throws
        strSavingThrowToggle.isOn = currCharacter.strSavingThrow;
        dexSavingThrowToggle.isOn = currCharacter.dexSavingThrow;
        conSavingThrowToggle.isOn = currCharacter.conSavingThrow;
        intSavingThrowToggle.isOn = currCharacter.intSavingThrow;
        wisSavingThrowToggle.isOn = currCharacter.wisSavingThrow;
        charismaSavingThrowToggle.isOn = currCharacter.charismaSavingThrow;

        // Skill checks
        acrobaticsCheckToggle.isOn = currCharacter.acrobaticsCheck;
        slightOfHandCheckToggle.isOn = currCharacter.slightOfHandCheck;
        stealthCheckToggle.isOn = currCharacter.stealthCheck;

        arcanaCheckToggle.isOn = currCharacter.arcanaCheck;
        historyCheckToggle.isOn = currCharacter.historyCheck;
        investCheckToggle.isOn = currCharacter.investCheck;
        natureCheckToggle.isOn = currCharacter.natureCheck;
        religionCheckToggle.isOn = currCharacter.religionCheck;

        animalCheckToggle.isOn = currCharacter.animalCheck;
        insightCheckToggle.isOn = currCharacter.insightCheck;
        medicineCheckToggle.isOn = currCharacter.medicineCheck;
        percepCheckToggle.isOn = currCharacter.percepCheck;
        survivalCheckToggle.isOn = currCharacter.survivalCheck;

        deceptionCheckToggle.isOn = currCharacter.deceptionCheck;
        intimCheckToggle.isOn = currCharacter.intimCheck;
        performanceCheckToggle.isOn = currCharacter.performanceCheck;
        persuassionCheckToggle.isOn = currCharacter.persuassionCheck;
    }

    public void LoadCharacter()
    {
        if (!Directory.Exists(CharactersFolder))
        {
            Directory.CreateDirectory(CharactersFolder);
        }

        string initialDir = Path.GetFullPath(CharactersFolder);

        OpenFileDialog dialog = new OpenFileDialog();
        dialog.InitialDirectory = initialDir;
        dialog.Filter = "JSON Character (*.json)|*.json";
        dialog.Title = "Select Character File";

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _pendingCharacterPath = dialog.FileName;
            Debug.Log($"Selected Character File: {_pendingCharacterPath}");

            if (File.Exists(_pendingCharacterPath))
            {
                string json = File.ReadAllText(_pendingCharacterPath);
                currCharacter = JsonUtility.FromJson<CharacterData>(json);
                UpdateUIFromCharacterData(); // Update TMP fields and toggles

                // Reset TMP focus so input fields work properly
                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }

                Debug.Log("Character loaded successfully.");
            }
            else
            {
                Debug.LogWarning("Selected file does not exist: " + _pendingCharacterPath);
            }
        }
        else
        {
            Debug.Log("Load canceled by user.");
        }
    }

    public void SetPortraitLocation(string path)
    {
        characterPortraitLocation = path;
    }

    // Helper function
    private Sprite LoadSpriteFromPath(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogWarning("Invalid portrait path: " + path);
            return null;
        }

        byte[] imageData = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(imageData))
        {
            Debug.LogError("Failed to load image from: " + path);
            return null;
        }

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}