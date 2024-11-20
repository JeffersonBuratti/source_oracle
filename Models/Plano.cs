namespace Oracle_Consummer.Models
{
    public class Plano
    {
        public string Nnumeplan { get; set; }
        public string Cdescplan { get; set; }

        public string Ccodiplan { get; set; }
        public List<TabelaPreco> TabelasDePreco { get; set; } // Relacionamento com TabelaPreco
    }
}
