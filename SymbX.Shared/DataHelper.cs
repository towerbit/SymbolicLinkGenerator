namespace SymbX.Shared
{
    internal class DataHelper
    {
        public SymbXItem[] Items { get; set; }

        public DataHelper()
        {
            Items = new SymbXItem[0];
        }

        public DataHelper(string json)
        {
            //this.Items = JsonConvert.DeserializeObject<dtoSLGItem[]>(json);
            this.Items = JsonSerializer.Deserialize<SymbXItem[]>(json);
        }

        //public string ToJson() => JsonConvert.SerializeObject(this.Items);
        public string ToJson() => JsonSerializer.Serialize(this.Items);
    }
}