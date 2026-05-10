namespace Midas.API.DTOs
{
    public class HateoasResponse<T>
    {
        public T Data { get; set; }
        public List<HateoasLink> Links { get; set; } = new List<HateoasLink>();

        public HateoasResponse(T data)
        {
    Data = data;
        }
    }

    public class HateoasPagedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasNext => CurrentPage < TotalPages;
 public bool HasPrevious => CurrentPage > 1;
      public List<HateoasLink> Links { get; set; } = new List<HateoasLink>();

        public HateoasPagedResponse()
        {
        }

        public HateoasPagedResponse(List<T> data, int totalRecords, int currentPage, int pageSize)
        {
          Data = data;
 TotalRecords = totalRecords;
            CurrentPage = currentPage;
      PageSize = pageSize;
    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
     }
    }
}