using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyRotation : FuzzyOperations
{
    public IaCarro Carro;

    //Declare Fuzzy Membership Curves
    public AnimationCurve Velocidade_Esquerda;
    public AnimationCurve Velocidade_Centro;
    public AnimationCurve Velocidade_Direita;

    public AnimationCurve Distancia_E_Baixa;
    public AnimationCurve Distancia_E_Media;
    public AnimationCurve Distancia_E_Alta;

    public AnimationCurve Distancia_D_Baixa;
    public AnimationCurve Distancia_D_Media;
    public AnimationCurve Distancia_D_Alta;

    public AnimationCurve Aceleracao_Esquerdissima;
    public AnimationCurve Aceleracao_Esquerda;
    public AnimationCurve Aceleracao_Centro;
    public AnimationCurve Aceleracao_Direita;
    public AnimationCurve Aceleracao_Direitissima;

    //Tables to store the membership of a given value to each fuzzy set
    private Dictionary<string, float> Dicionario_Velocidade;
    private Dictionary<string, float> Dicionario_Distancia_E;
    private Dictionary<string, float> Dicionario_Distancia_D;
    private Dictionary<string, float> Dicionario_Aceleracao;

    //Table with the Fuzzy Rules to be Evaluated
    public Dictionary<string, string> Regras;

    //Defuzzified output
    private float aceleracaoFinal;
    public float maxVelocidade;
    public float maxDistancia;
    private float maxAceleracao; 

    //Fuctions to fuzzify each linguistic variable
    public void FuzzifyVelocidade(float inputValue)
    {
        Dicionario_Velocidade["Esquerda"] = Velocidade_Esquerda.Evaluate(inputValue);
        Dicionario_Velocidade["Centro"] = Velocidade_Centro.Evaluate(inputValue);
        Dicionario_Velocidade["Direita"] = Velocidade_Direita.Evaluate(inputValue);
    }

    public void FuzzifyDistanciaE(float inputValue)
    {
        Dicionario_Distancia_E["Baixa"] = Distancia_E_Baixa.Evaluate(inputValue);
        Dicionario_Distancia_E["Media"] = Distancia_E_Media.Evaluate(inputValue);
        Dicionario_Distancia_E["Alta"] = Distancia_E_Alta.Evaluate(inputValue);
    }

    public void FuzzifyDistanciaD(float inputValue)
    {
        Dicionario_Distancia_D["Baixa"] = Distancia_E_Baixa.Evaluate(inputValue);
        Dicionario_Distancia_D["Media"] = Distancia_E_Media.Evaluate(inputValue);
        Dicionario_Distancia_D["Alta"] = Distancia_E_Alta.Evaluate(inputValue);
    }

    public void SetAllValues()
    {
        // Set values for each membership function
        SetFuzzify(new[] { new Vector2(-maxVelocidade, 1f), new Vector2(0, 0f) }, Velocidade_Esquerda);
        SetFuzzify(new[] { new Vector2(-maxVelocidade, 0f), new Vector2(0, 1f), new Vector2(maxVelocidade, 0) }, Velocidade_Centro);
        SetFuzzify(new[] { new Vector2(0, 0f), new Vector2(maxVelocidade, 1f) }, Velocidade_Direita);

        SetFuzzify(new[] { new Vector2(0f, 1f), new Vector2(maxDistancia / 2, 0f) }, Distancia_E_Baixa);
        SetFuzzify(new[] { new Vector2(0f, 0f), new Vector2(maxDistancia / 2, 1f), new Vector2(maxDistancia, 0) }, Distancia_E_Media);
        SetFuzzify(new[] { new Vector2(maxDistancia / 2, 0f), new Vector2(maxDistancia, 1f) }, Distancia_E_Alta);

        SetFuzzify(new[] { new Vector2(0f, 1f), new Vector2(maxDistancia / 2, 0f) }, Distancia_D_Baixa);
        SetFuzzify(new[] { new Vector2(0f, 0f), new Vector2(maxDistancia / 2, 1f), new Vector2(maxDistancia, 0) }, Distancia_D_Media);
        SetFuzzify(new[] { new Vector2(maxDistancia / 2, 0f), new Vector2(maxDistancia, 1f) }, Distancia_D_Alta);

        SetFuzzify(new[] { new Vector2((-3 * maxAceleracao / 2), 0f), new Vector2((-2 * maxAceleracao / 2), 1f), new Vector2((-1 * maxAceleracao / 2), 0f) }, Aceleracao_Esquerdissima);
        SetFuzzify(new[] { new Vector2((-2 * maxAceleracao / 2), 0f), new Vector2((-1 * maxAceleracao / 2), 1f), new Vector2(0, 0f) }, Aceleracao_Esquerda);
        SetFuzzify(new[] { new Vector2((-1 * maxAceleracao / 2), 0f), new Vector2(0, 1f), new Vector2((1 * maxAceleracao / 2), 0) }, Aceleracao_Centro);
        SetFuzzify(new[] { new Vector2(0, 0f), new Vector2((1 * maxAceleracao / 2), 1f), new Vector2((2 * maxAceleracao / 2), 0f) }, Aceleracao_Direita);
        SetFuzzify(new[] { new Vector2((1 * maxAceleracao / 2), 0f), new Vector2((2 * maxAceleracao / 2), 1f), new Vector2((3 * maxAceleracao / 2), 0f) }, Aceleracao_Direitissima);
    }

    public void SetAllRules()
    {
        //Initialize RuleBase dictionary and fill in the rules ( distancia E + distancia D + velocidade)
        Regras = new Dictionary<string, string>();

        Regras.Add("AltaAltaEsquerda", "Direita");
        Regras.Add("AltaAltaCentro", "Centro");
        Regras.Add("AltaAltaDireita", "Esquerda");

        Regras.Add("AltaMediaEsquerda", "Esquerda");
        Regras.Add("AltaMediaCentro", "Esquerda");
        Regras.Add("AltaMediaDireita", "Esquerdissima");

        Regras.Add("AltaBaixaEsquerda", "Esquerdissima");
        Regras.Add("AltaBaixaCentro", "Esquerdissima");
        Regras.Add("AltaBaixaDireita", "Esquerdissima");

        Regras.Add("MediaAltaEsquerda", "Direitissima");
        Regras.Add("MediaAltaCentro", "Direita");
        Regras.Add("MediaAltaDireita", "Direita");

        Regras.Add("MediaMediaEsquerda", "Direita");
        Regras.Add("MediaMediaCentro", "Centro");
        Regras.Add("MediaMediaDireita", "Esquerda");

        Regras.Add("MediaBaixaEsquerda", "Esquerda");
        Regras.Add("MediaBaixaCentro", "Esquerda");
        Regras.Add("MediaBaixaDireita", "Esquerdissima");

        Regras.Add("BaixaAltaEsquerda", "Direitissima");
        Regras.Add("BaixaAltaCentro", "Direitissima");
        Regras.Add("BaixaAltaDireita", "Direitissima");

        Regras.Add("BaixaMediaEsquerda", "Direitissima");
        Regras.Add("BaixaMediaCentro", "Direita");
        Regras.Add("BaixaMediaDireita", "Direita");

        Regras.Add("BaixaBaixaEsquerda", "Direita");
        Regras.Add("BaixaBaixaCentro", "Centro");
        Regras.Add("BaixaBaixaDireita", "Esquerda");

    }

    public void InitializeValues()
    {
        //Intialize membership values
        Dicionario_Velocidade = new Dictionary<string, float>();
        Dicionario_Distancia_E = new Dictionary<string, float>();
        Dicionario_Distancia_D = new Dictionary<string, float>();
        Dicionario_Aceleracao = new Dictionary<string, float>();

        Dicionario_Velocidade.Add("Esquerda", 0f);
        Dicionario_Velocidade.Add("Centro", 0f);
        Dicionario_Velocidade.Add("Direita", 0f);

        Dicionario_Distancia_E.Add("Baixa", 0f);
        Dicionario_Distancia_E.Add("Media", 0f);
        Dicionario_Distancia_E.Add("Alta", 0f);

        Dicionario_Distancia_D.Add("Baixa", 0f);
        Dicionario_Distancia_D.Add("Media", 0f);
        Dicionario_Distancia_D.Add("Alta", 0f);

        Dicionario_Aceleracao.Add("Esquerdissima", 0f);
        Dicionario_Aceleracao.Add("Esquerda", 0f);
        Dicionario_Aceleracao.Add("Centro", 0f);
        Dicionario_Aceleracao.Add("Direita", 0f);
        Dicionario_Aceleracao.Add("Direitissima", 0f);
    }

    //Clean any trash membership values that might be left from a previous operation
    public void ResetValues()
    {
        Dicionario_Velocidade["Esquerda"] = 0f;
        Dicionario_Velocidade["Centro"] = 0f;
        Dicionario_Velocidade["Direita"] = 0f;

        Dicionario_Distancia_E["Baixa"] = 0f;
        Dicionario_Distancia_E["Media"] = 0f;
        Dicionario_Distancia_E["Alta"] = 0f;

        Dicionario_Distancia_D["Baixa"] = 0f;
        Dicionario_Distancia_D["Media"] = 0f;
        Dicionario_Distancia_D["Alta"] = 0f;

        Dicionario_Aceleracao["Esquerdissima"] = 0f;
        Dicionario_Aceleracao["Esquerda"] = 0f;
        Dicionario_Aceleracao["Centro"] = 0f;
        Dicionario_Aceleracao["Direita"] = 0f;
        Dicionario_Aceleracao["Direitissima"] = 0f;

    }

    //Iterate over all possible memberships and process the rules present
    public void EvaluateRuleBase()
    {
        foreach (var keyA in Dicionario_Velocidade.Keys)
        {
            foreach (var keyE in Dicionario_Distancia_E.Keys)
            {
                foreach (var keyD in Dicionario_Distancia_D.Keys)
                {
                    if (Dicionario_Distancia_D[keyD] > 0 && Dicionario_Distancia_E[keyE] > 0 && Dicionario_Velocidade[keyA] > 0)
                    {
                        string ResultadoAceleracao = Regras[keyE + keyD + keyA];
                        Dicionario_Aceleracao[ResultadoAceleracao] = Mathf.Max(Mathf.Min(Dicionario_Velocidade[keyA], Dicionario_Distancia_E[keyE], Dicionario_Distancia_D[keyD]), Dicionario_Aceleracao[ResultadoAceleracao]);
                    }
                }

            }
        }

    }

    //Defuzzify the fuzzy output into a single value using the Sum of Centers method
    public float DefuzzifyAceleracao()
    {
        EvaluateRuleBase();

        List<float> Areas = new List<float>();
        List<float> Centers = new List<float>();

        // For each possible membership, calculate the area and its center
        foreach (var keyP in Dicionario_Aceleracao.Keys)
        {
            float U_a = Dicionario_Aceleracao[keyP];

            float area = 0;
            float center = 0;
            if (keyP == "Direitissima")
            {
                area = CalculateTrapezoidArea(Aceleracao_Direitissima, U_a);
                center = CalculateCenter(Aceleracao_Direitissima, U_a);
            }
            else if (keyP == "Direita")
            {
                area = CalculateTrapezoidArea(Aceleracao_Direita, U_a);
                center = CalculateCenter(Aceleracao_Direita, U_a);
            }
            else if (keyP == "Centro")
            {
                area = CalculateTrapezoidArea(Aceleracao_Centro, U_a);
                center = CalculateCenter(Aceleracao_Centro, U_a);
            }
            else if (keyP == "Esquerda")
            {
                area = CalculateTrapezoidArea(Aceleracao_Esquerda, U_a);
                center = CalculateCenter(Aceleracao_Esquerda, U_a);
            }
            else
            {
                area = CalculateTrapezoidArea(Aceleracao_Esquerdissima, U_a);
                center = CalculateCenter(Aceleracao_Esquerdissima, U_a);
            }

            Areas.Add(area);
            Centers.Add(center);
        }

        float numerator = 0;
        float den = 0;

        //Perform weighted average given area size of each membership
        for (int i = 0; i < Areas.Count; i++)
        {
            numerator += Areas[i] * Centers[i];
            den += Areas[i];
        }

        return aceleracaoFinal = numerator / den;
    }

    private void Awake()
    {

        maxAceleracao = 10 * maxVelocidade;
        Carro.SetDistanciaDirCol(maxDistancia);
        Carro.SetDistanciaEsqCol(maxDistancia);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetAllValues();
        SetAllRules();

        InitializeValues();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCar();
    }

    public void MoveCar()
    {
        ResetValues();

        FuzzifyVelocidade(Carro.GetRotacao());
        FuzzifyDistanciaE(Carro.GetDistanciaEsqCol());
        FuzzifyDistanciaD(Carro.GetDistanciaDirCol());

        EvaluateRuleBase();
        Carro.SetRotacao(DefuzzifyAceleracao());
        
    }

}
