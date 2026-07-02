namespace InsurancePolicyManager.Application.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Itens { get; set; } = [];
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int Total { get; set; }
}
