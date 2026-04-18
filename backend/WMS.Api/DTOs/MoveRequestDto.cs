namespace WMS.Api.DTOs;

public record MoveRequestDto(
    int ItemId,
    int? FromLocationId,
    int? ToLocationId,
    decimal Quantity,
    string? Note
);