namespace CloudCompare.Helpers
{
    public static class UiHelper
    {
        // Returns a CSS class used to colour a provider (AWS orange, Azure blue, GCP green)
        public static string ProviderClass(string name)
        {
            var n = (name ?? "").ToLower();
            if (n.Contains("amazon") || n.Contains("aws")) return "p-aws";
            if (n.Contains("azure") || n.Contains("microsoft")) return "p-azure";
            if (n.Contains("google") || n.Contains("gcp")) return "p-gcp";
            return "p-other";
        }
    }
}
