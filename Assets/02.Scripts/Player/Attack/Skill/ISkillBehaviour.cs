using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillBehaviour
{
    void Execute(in SkillContext ctx);
}
