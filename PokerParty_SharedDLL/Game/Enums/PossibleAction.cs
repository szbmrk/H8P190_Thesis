using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public enum PossibleAction
    {
        SMALL_BLIND_BET, //kötelező tét
        BIG_BLIND_BET, //kötelező tét
        FOLD, //bedobás
        CHECK, //passzolás
        BET, //nyitás
        CALL, //tartás
        RAISE, //emelés
        ALL_IN //all in
    }
}
