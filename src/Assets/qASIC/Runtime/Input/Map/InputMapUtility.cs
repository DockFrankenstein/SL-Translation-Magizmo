namespace qASIC.Input.Map
{
    public static class InputMapUtility
    {
        public static bool IsGuidBroken<T>(InputMap map, string guid) where T : InputMapItem =>
            !string.IsNullOrWhiteSpace(guid) && map.GetItem<T>(guid) == null;

        public static bool TryGetItemFromPath(InputMap map, string groupName, string itemName, out InputMapItem item)
        {
            item = null;

            //REMOVEME: this is a temp fix, because the map gets set to null when recompiling during runtime
            if (map == null)
                return false;

            if (!map.TryGetGroup(groupName, out InputGroup group))
                return false;

            if (!group.TryGetItem(itemName, out item))
                return false;

            return true;
        }

        public static bool TryGetItemFromPath<T>(InputMap map, string groupName, string itemName, out InputMapItem<T> item)
        {
            item = null;

            if (!TryGetItemFromPath(map, groupName, itemName, out InputMapItem mapItem))
                return false;

            if (mapItem.ValueType != typeof(T))
                return false;

            item = mapItem as InputMapItem<T>;
            return true; 
        } 
    }
}