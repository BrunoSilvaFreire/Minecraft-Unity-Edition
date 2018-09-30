namespace Minecraft.Scripts.Utility {
    public static class IndexingUtility {
        
        public static int IndexOf(int x, int y, int z, int width, int height) {
            return x + width * (y + height * z);
        }

        public static int IndexOf(byte x, byte y, byte z, byte width, byte height) {
            return x + width * (y + height * z);
        }
    }
}