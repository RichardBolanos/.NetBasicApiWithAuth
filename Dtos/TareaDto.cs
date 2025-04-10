namespace GestionTareasApi.Dtos
{
    public class TareaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Completada { get; set; } = false;
    }
}
