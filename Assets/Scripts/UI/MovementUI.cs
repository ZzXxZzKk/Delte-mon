﻿
namespace BattleDelts.UI
{
    public class MovementUI : UIScreen
    {

        public override void Open()
        {
            root.SetActiveIfChanged(true);
        }

        public override void Close()
        {
            root.SetActiveIfChanged(false);
        }
    }
}