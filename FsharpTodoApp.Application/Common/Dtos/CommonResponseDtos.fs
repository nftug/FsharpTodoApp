namespace FsharpTodoApp.Application.Common.Dtos

type ItemCreatedResponseDto(itemId) =
    member val ItemId: System.Guid = itemId with get, set

type AuditDto(createdAt, createdBy, updatedAt, updatedBy) =
    member val CreatedAt: System.DateTime = createdAt with get, set
    member val CreatedBy: string = createdBy with get, set
    member val UpdatedAt: System.DateTime option = updatedAt with get, set
    member val UpdatedBy: string option = updatedBy with get, set

[<AbstractClass>]
type PaginatedResponseDto<'T>(items: 'T list, totalCount: int, pageSize: int, currentPage: int) =
    member val TotalCount: int = totalCount with get, set
    member val PageSize: int = pageSize with get, set
    member val CurrentPage: int = currentPage with get, set
    member this.TotalPages: int = (this.TotalCount + this.PageSize - 1) / this.PageSize
    member this.HasNextPage: bool = this.CurrentPage < this.TotalPages
    member this.HasPreviousPage: bool = this.CurrentPage > 1
    member val Items: 'T list = items with get, set
