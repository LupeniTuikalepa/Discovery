using LTX.ChanneledProperties;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace Discovery.Core
{
    public static class GameController
    {
        [System.Serializable]
        public struct CursorData
        {
            [SerializeField]
            public Texture2D cursorTexture;
            [SerializeField]
            public Vector2 cursorHotSpot;
            [SerializeField]
            public CursorMode mode;
        }

        private static ChannelKey gameKey;

        public static Priority<CursorData> CurrentCursor { get; private set; }
        public static Priority<CursorLockMode> CursorLock { get; private set; }
        public static Priority<bool> IsCursorVisible { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            gameKey = ChannelKey.GetUniqueChannelKey("GameController");

            CursorLock = new Priority<CursorLockMode>(CursorLockMode.Confined);
            IsCursorVisible = new Priority<bool>(true);
            CurrentCursor = new Priority<CursorData>(new CursorData()
            {
                cursorTexture = null,
                cursorHotSpot = Vector2.zero,
                mode = CursorMode.Auto,
            });

            CursorLock.AddOnValueChangeCallback(ctx => Cursor.lockState = ctx, true);
            IsCursorVisible.AddOnValueChangeCallback(ctx => Cursor.visible = ctx, true);

            CurrentCursor.AddOnValueChangeCallback(ctx =>
                Cursor.SetCursor(ctx.cursorTexture, ctx.cursorHotSpot, ctx.mode), true);
        }


    }
}