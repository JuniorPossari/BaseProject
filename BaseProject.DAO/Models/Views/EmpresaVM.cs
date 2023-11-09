
using BaseProject.Util;

namespace BaseProject.DAO.Models.Views
{
    public class EmpresaVM
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public string DataCadastro { get; set; }
        public bool Ativa { get; set; }
        public Usuario[] Usuario { get; set; }
        public EmpresaLogo EmpresaLogo { get; set; }
        public string LogoDataUrl { get; set; }

        public EmpresaVM() { }

        public EmpresaVM(Empresa model)
        {
            Id = model.Id;
            RazaoSocial = model.RazaoSocial;
            NomeFantasia = model.NomeFantasia;
            CNPJ = model.CNPJ.MaskCNPJ();
            DataCadastro = model.DataCadastro.ToString("dd/MM/yyyy");
            Ativa = model.Ativa;
            Usuario = model.Usuario.ToArray();
            EmpresaLogo = model.EmpresaLogo;
            LogoDataUrl = model.EmpresaLogo != null ? ("data:" + model.EmpresaLogo.Tipo + ";base64," + model.EmpresaLogo.Base64) : "";
        }

    }
}