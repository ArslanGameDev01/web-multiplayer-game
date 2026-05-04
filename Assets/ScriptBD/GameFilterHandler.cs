using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GameFilterHandler : MonoBehaviour
{
    [SerializeField] private QuestionData gameScriptable; // Reference to QuestionData ScriptableObject
    [SerializeField] private APIDataFetching dataFetching; // Reference to APIDataFetching for contentObj
    [SerializeField] private TMP_Dropdown ageFilterDropdown; // Dropdown for age ranges

    [SerializeField] private TMP_Dropdown categoryFilterDropdown; // Dropdown for categories

    // [SerializeField] private Toggle sponsorFilterToggle; // Toggle for sponsor filter
    [SerializeField] private GameObject ageText; // Template GameObject with TMP_Text for age display

    [SerializeField]
    private GameObject categoryText; // Template GameObject with TMP_Text for category display (fixed typo)

    //  [SerializeField] private GameObject sponserSelect;
    [SerializeField] private GameObject gameFilterError;
    public int currentActiveGame;
    public List<agefilter> ageFilters;
    public List<string> ageFilterOptions; // List of age range labels (e.g., "No Filter", "5-10")
    public List<string> categoryFilterOptions; // List of category labels (e.g., "No Filter", "Math")
    public int selectedAgeIndex = 0; // Current age filter index (0 = no filter)
    public int selectedCategoryIndex = 0; // Current category filter index (0 = no filter)
    private bool isSponsorFilterActive = false; // Sponsor filter state

    void Start()
    {
    }

    public void UpdatedFilater()
    {
        selectedAgeIndex = 0;
        selectedCategoryIndex = 0;

        if (gameScriptable == null || dataFetching == null || ageFilterDropdown == null ||
            categoryFilterDropdown == null)
        {
            Debug.LogError("Missing components in GameFilterHandler Inspector!");
            return;
        }

        InitializeFilters();
        ageFilterDropdown.onValueChanged.AddListener(OnAgeFilterChanged);
        categoryFilterDropdown.onValueChanged.AddListener(OnCategoryFilterChanged);
        ApplyFilters();
    }

    // Initialize age and category filter dropdowns
    private void InitializeFilters()
    {
        ageFilterOptions = new List<string> { "All" };
        ageFilters = new List<agefilter>();

        if (gameScriptable.root?.data?.ageranges != null)
        {
            foreach (var ageRange in gameScriptable.root.data.ageranges)
            {
                ageFilterOptions.Add($"{ageRange.min_age}-{ageRange.max_age}");
                ageFilters.Add(new agefilter { min_age = ageRange.min_age, max_age = ageRange.max_age });
            }
        }

        ageFilterDropdown.ClearOptions();
        ageFilterDropdown.AddOptions(ageFilterOptions);
        ageFilterDropdown.value = 0;

        categoryFilterOptions = new List<string> { "All" };
        if (gameScriptable.root?.data?.categories != null)
        {
            foreach (var category in gameScriptable.root.data.categories)
            {
                categoryFilterOptions.Add(category.title);
            }
        }

        categoryFilterDropdown.ClearOptions();
        categoryFilterDropdown.AddOptions(categoryFilterOptions);
        categoryFilterDropdown.value = 0;

        ApplyFilters();
    }

    // Handle age filter change
    private void OnAgeFilterChanged(int index)
    {
        selectedAgeIndex = index;
        ApplyFilters();
    }

    // Handle category filter change
    private void OnCategoryFilterChanged(int index)
    {
        selectedCategoryIndex = index;
        ApplyFilters();
    }

    // Handle sponsor filter change
    // private void OnSponsorFilterChanged(bool isOn)
    // {
    //     isSponsorFilterActive = isOn;
    //     sponserSelect.SetActive(isSponsorFilterActive);
    //     ApplyFilters();
    // }

    // Apply filters to update displayed games
    private void ApplyFilters()
    {
        if (gameScriptable.root?.data?.games == null)
        {
            Debug.LogWarning("No game data available for filtering!");
            return;
        }

        currentActiveGame = 0;
        categoryText.SetActive(selectedCategoryIndex == 0);
        ageText.SetActive(selectedAgeIndex == 0);

        if (selectedAgeIndex == 0)
        {
            ageFilterDropdown.captionText.text = "";
        }

        if (selectedCategoryIndex == 0)
        {
            categoryFilterDropdown.captionText.text = "";
            categoryFilterDropdown.captionText.color = new Color(1f, 1f, 1f, 0);
        }
        else
        {
            categoryFilterDropdown.captionText.color = Color.black;
        }

        var gameObjects = dataFetching.contentObj;
        if (gameObjects == null || gameObjects.Count == 0)
        {
            Debug.LogWarning("No game objects to filter!");
            return;
        }

        for (int i = 0; i < gameObjects.Count; i++)
        {
            var gameObj = gameObjects[i];
            if (gameObj == null) continue;

            var gameData = gameScriptable.root.data.games[i];
            bool shouldShow = true;

            // Apply age filter
            if (selectedAgeIndex > 0 && shouldShow)
            {
                var selectedAge = ageFilters[selectedAgeIndex - 1];
                shouldShow &= gameData.ageranges.Any(ar =>
                    ar.min_age == selectedAge.min_age && ar.max_age == selectedAge.max_age);
            }

            // Apply category filter
            if (selectedCategoryIndex > 0 && shouldShow)
            {
                var selectedCategoryTitle = categoryFilterOptions[selectedCategoryIndex];
                shouldShow &= gameData.categories.Any(c => c.title == selectedCategoryTitle);
            }

            if (shouldShow) currentActiveGame++;

            gameObj.gameObject.SetActive(shouldShow);
        }

        gameFilterError.SetActive(currentActiveGame == 0);
        int visibleGames = gameObjects.Count(g => g.gameObject.activeSelf);
        GamePlayHandler.instance.ForScrollerObj(visibleGames > 2);
    }

    // Public method to reset filters
    public void ResetFilters()
    {
        ageFilterDropdown.value = 0;
        categoryFilterDropdown.value = 0;
        //  sponsorFilterToggle.isOn = false;
        selectedAgeIndex = 0;
        selectedCategoryIndex = 0;
        isSponsorFilterActive = false;
        ApplyFilters();
    }

    [System.Serializable]
    public class agefilter
    {
        public string min_age;
        public string max_age;
    }
}