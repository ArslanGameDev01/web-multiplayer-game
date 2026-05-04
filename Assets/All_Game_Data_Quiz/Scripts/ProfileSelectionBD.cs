//using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSelectionBD : MonoBehaviour
{
    public FusionConnector FusionConnectorData;

    [Header("Current Selection UI")]
    public TMP_Text currentNameText;
    public Image currentAvatarImage;

    public TMP_Text selectedNameText;
    public Image selectedAvatarImage;

    [Header("Name Buttons")]
    public Button[] nameButtons;
    public Sprite selectedSprite;
    public Sprite nonSelectedSprite;

    [Header("Avatar Buttons")]
    public Button[] avatarButtons;

    private string selectedName = "";
    public static int selectedAvatarIndex = -1;

    void Start()
    {
        // Setup name buttons
        for (int i = 0; i < nameButtons.Length; i++)
        {
            int index = i;
            nameButtons[i].onClick.AddListener(() => OnNameSelected(index));
        }

        // Setup avatar buttons
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            int index = i;
            avatarButtons[i].onClick.AddListener(() => OnAvatarSelected(index));
        }
        // Do initial random selection
        RandomizeSelection();
    }
    void RandomizeSelection()
    {
        int randomNameIndex = Random.Range(0, nameButtons.Length);
        int randomAvatarIndex = Random.Range(0, avatarButtons.Length);

        OnNameSelected(randomNameIndex);
        OnAvatarSelected(randomAvatarIndex);
    }
    void OnNameSelected(int index)
    {
        TMP_Text text = nameButtons[index].GetComponentInChildren<TMP_Text>();
        selectedName = text.text;

        string finalName = selectedName + " " + GetAnimalName();
        currentNameText.text = finalName;
        selectedNameText.text = finalName;

        
       // PhotonNetwork.NickName = finalName; // also set nickname for Photon
        PlayerProfileData.playerName = finalName; // store for other uses
        FusionConnectorData.LocalPlayerName= finalName;
        
        // Button visuals
        for (int i = 0; i < nameButtons.Length; i++)
            nameButtons[i].image.sprite = (i == index) ? selectedSprite : nonSelectedSprite;
    }

    void OnAvatarSelected(int index)
    {
        selectedAvatarIndex = index;

        Image avatarImage = avatarButtons[index].GetComponent<Image>();
        currentAvatarImage.sprite = avatarImage.sprite;
        selectedAvatarImage.sprite = avatarImage.sprite;

        PlayerProfileData.avatarIndex = index;

        for (int i = 0; i < avatarButtons.Length; i++)
        {
            Transform tick = avatarButtons[i].transform.GetChild(0);
            tick.gameObject.SetActive(i == index);
        }

        string finalName = selectedName + " " + GetAnimalName();
        //PhotonNetwork.NickName = finalName;
        PlayerProfileData.playerName = finalName;
        FusionConnectorData.LocalPlayerName= finalName;
        FusionConnectorData.avatarIndexAnimal = index;
        
       // TriviaPlayer.LocalPlayer.AvatarIndex = index;
        
        currentNameText.text = finalName;
        selectedNameText.text = finalName;
    }
    string GetAnimalName()
    {
        string[] animalNames = { "Alligator", "Armadillo", "Badger", "Cobra", "Dolphin", "Flamingo", "Fox", "Frog",
        "Gecko", "Giraffe", "Goat", "Hedgehog", "Humming Bird", "Jelly Fish", "Kangaroo", "Koala", "Leopard", "Llama",
        "Lobster", "Manatee", "Meerkat", "Moose", "Narwhal", "Otter", "Owl", "Panda", "Penguin", "Platypus", "Sloth",
        "Squirrel", "Starfish", "Tiger", "Turtle", "Wolf", "Wombat"};

        if (selectedAvatarIndex >= 0 && selectedAvatarIndex < animalNames.Length)
            return animalNames[selectedAvatarIndex];
        return "Animal";
    }
}
