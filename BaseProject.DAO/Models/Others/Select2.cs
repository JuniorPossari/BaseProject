namespace BaseProject.DAO.Models.Others
{
	public class S2Param
    {
        public string Term { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int IPP { get; set; } = 30;

        public int InitialPosition()
        {
            return (Page - 1) * IPP;
        }

        public int ItensPerPage()
        {
            return IPP;
        }

        public string SearchValue()
        {
            return Term;
        }

        public bool More(int total)
        {
            return (Page * IPP) < total;
        }

    }

    public class S2Result
    {
        public S2Option[] Itens { get; set; } = Array.Empty<S2Option>();
        public int Total { get; set; } = 0;
    }

    public class S2Option
    {
        public int Id { get; set; }
        public string Text { get; set; }        
    }
}
