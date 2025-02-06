
namespace Localization
{
    public static class ConstantStrings
    {
        public const string NEW_WEAPON    = "new weapon";
        public const string NEW_POWER     = "new power";
        public const string NEW_UPGRADE   = "upgrade";
    }

    //public class LoadLocalizedFiles
    //{
    //    public static void LoadFile<T>(string idFile, Action<T> onDataLoaded)
    //    {
    //        try
    //        {
    //            TextAsset textAsset = Lean.Localization.LeanLocalization.GetTranslationObject<TextAsset>(idFile)
    //                                  ?? Resources.Load<TextAsset>(idFile);

    //            if (textAsset == null)
    //            {
    //                Debug.LogError("Error loading dialogue data: " + idFile);
    //                return;
    //            }

    //            string json = textAsset.text;
    //            T data = JsonUtility.FromJson<T>(json);

    //            if (data == null)
    //            {
    //                throw new InvalidOperationException($"Failed to deserialize dialogue data for ID: {idFile}");
    //            }

    //            onDataLoaded(data);
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError("Error loading dialogue data: " + ex.Message);
    //            throw;
    //        }
    //    }

    //    public static string GetLocalizedText(string id, string keyToReplace = "", string replaceText = "")
    //    {
    //        string localizedText = LeanLocalization.GetTranslationText(id);

    //        if (string.IsNullOrEmpty(localizedText))
    //        {
    //            Debug.LogWarning($"NO FIND LOCALIZATION ID: {id}");
    //            return id;
    //        }
    //        if (!string.IsNullOrEmpty(replaceText) && !string.IsNullOrEmpty(keyToReplace))
    //            localizedText = localizedText.Replace(keyToReplace, replaceText);

    //        //In case dont recognize emojis or symbols
    //        localizedText = localizedText.Replace("\uFE0F", "");


    //        return localizedText;
    //    }
    //}
}

