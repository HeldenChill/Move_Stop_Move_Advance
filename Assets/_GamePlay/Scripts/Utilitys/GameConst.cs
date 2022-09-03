using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameConst
{

    public enum Type
    {
        Character = 0,
        Sensor = 1,
        Model = 2
    }

    public static readonly string ANIM_IS_IDLE = "IsIdle";
    public static readonly string ANIM_IS_DEAD = "IsDead";
    public static readonly string ANIM_IS_ATTACK = "IsAttack";
    public static readonly string ANIM_IS_WIN = "IsWin";
    public static readonly string ANIM_IS_DANCE = "IsDance";
    public static readonly string ANIM_IS_ULTI = "IsUlti";

    public static readonly int ANIM_IS_ATTACK_FRAMES = 33;
    public static readonly int ANIM_IS_SPECIAL_ATTACK_FRAMES = 47;

    public const float ANIM_IS_ATTACK_TIME = 1.03f;
    public const float ANIM_IS_DEAD_TIME = 2.06f;
    public const float INIT_CHARACTER_HEIGHT = 0.63f;
}
