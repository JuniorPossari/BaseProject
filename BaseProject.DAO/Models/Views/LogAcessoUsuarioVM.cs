
namespace BaseProject.DAO.Models.Views
{
    public class LogAcessoUsuarioVM
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string EnderecoIP { get; set; }
        public string Dispositivo { get; set; }
        public string Plataforma { get; set; }
        public string Navegador { get; set; }
        public bool Status { get; set; }
        public string Data { get; set; }
        public Usuario Usuario { get; set; }

        public LogAcessoUsuarioVM() { }

        public LogAcessoUsuarioVM(LogAcessoUsuario model)
        {
            Id = model.Id;
            IdUsuario = model.IdUsuario;
            EnderecoIP = model.EnderecoIP;
            Dispositivo = model.Dispositivo;
            Plataforma = model.Plataforma;
            Navegador = model.Navegador;
            Status = model.Status;
            Data = model.Data.ToString("dd/MM/yyyy hh:mm:ss");
            Usuario = model.IdUsuarioNavigation;
        }

    }
}