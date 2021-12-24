using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKPStateManager : MonoBehaviour
{
    JKPBaseState currentState;
    RandomCardPhase RandomCardstate = new RandomCardPhase();
    SpecialCardPhase SpecialCardState = new SpecialCardPhase();
    NormalCardPhase NormalCardState = new NormalCardPhase();
    ScoreConcludePhase ScoreConcludeState = new ScoreConcludePhase();

    void Start() 
    {
        currentState = RandomCardstate; 

        currentState.EnterState(this);
    }
    
    void Update() 
    {
        currentState.UpdateState(this);
    }

    void switchState(JKPBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}