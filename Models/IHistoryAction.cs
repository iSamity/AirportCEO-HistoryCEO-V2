namespace AirportCEOHistoryCEO.Models;

public interface IHistoryAction
{
    void Undo();
    void Redo();
}
