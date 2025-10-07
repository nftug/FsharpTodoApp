namespace FsharpTodoApp.Application.Common.Dtos

type ItemCreatedResponseDto = { ItemId: System.Guid }

module ItemCreatedResponseDto =
    open FsharpTodoApp.Domain.Common.Entities

    let create entityBase = { ItemId = entityBase.IdSet.PublicId }

type PaginatedResponseDto<'T> =
    { Items: 'T list
      TotalCount: int
      PageSize: int
      CurrentPage: int
      TotalPages: int
      HasNextPage: bool
      HasPreviousPage: bool }

module PaginatedResponseDto =
    let create (items: 'T list) (totalCount: int) (currentPage: int) (pageSize: int) : PaginatedResponseDto<'T> =
        let totalPages = (totalCount + pageSize - 1) / pageSize

        { Items = items
          TotalCount = totalCount
          PageSize = pageSize
          CurrentPage = currentPage
          TotalPages = totalPages
          HasNextPage = currentPage < totalPages
          HasPreviousPage = currentPage > 1 }
