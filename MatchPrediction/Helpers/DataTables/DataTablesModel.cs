namespace MatchPrediction.Helpers.DataTables
{
#pragma warning disable IDE1006 // Stili di denominazione
    public class DataTableAjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; } = default!;
        public Search search { get; set; } = default!;
        public List<Order> order { get; set; } = default!;
    }

    public class DataTableAjaxResponseModel<T>
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> data { get; set; } = default!; 
    }

    public class Column
    {
        public string data { get; set; } = default!;
        public string name { get; set; } = default!;
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; } = default!;
    }

    public class Search
    {
        public string value { get; set; } = default!;
        public string regex { get; set; } = default!;
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; } = default!;
    }
#pragma warning restore IDE1006 // Stili di denominazione
}
