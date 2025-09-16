namespace AirportCEOHistoryCEO.History.Models;

public interface IHistoryAction
{
    void Undo();
    void Redo();
}
