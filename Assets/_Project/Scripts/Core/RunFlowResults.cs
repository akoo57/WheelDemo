namespace WheelDemo.Core
{
    public enum RunCollectionResult
    {
        Success,
        InvalidState,
        MissingZoneDefinition,
        CollectionNotAllowed,
        SettlementFailed
    }

    public enum RunBombResult
    {
        Success,
        InvalidState
    }

    public enum RunReviveResult
    {
        Success,
        InvalidState,
        ServiceUnavailable,
        InsufficientCurrency
    }

    public enum RunRestartResult
    {
        Success,
        InvalidState
    }
}
