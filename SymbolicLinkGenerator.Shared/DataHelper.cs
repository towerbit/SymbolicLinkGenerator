namespace SymbolicLinkGenerator.Shared
{
    internal class DataHelper
    {
        public dtoSLGItem[] Items { get; set; }

        public DataHelper()
        {
            Items = new dtoSLGItem[0];
        }

        public DataHelper(string json)
        {
            //this.Items = JsonConvert.DeserializeObject<dtoSLGItem[]>(json);
            this.Items = JsonSerializer.Deserialize<dtoSLGItem[]>(json);
        }

        //public string ToJson() => JsonConvert.SerializeObject(this.Items);
        public string ToJson() => JsonSerializer.Serialize(this.Items);
    }
}