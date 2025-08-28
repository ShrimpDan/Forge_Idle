using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ITutorialAction //공통 인터페이스 사용함
{
    IEnumerator Execute(TutorialContext ctx, ActionData data);

}

public interface ITutorialCondition
{
    void Bind(TutorialContext ctx, ConditionData data);
    void Unbind(TutorialContext ctx);

    bool IsSatisfied();


}
