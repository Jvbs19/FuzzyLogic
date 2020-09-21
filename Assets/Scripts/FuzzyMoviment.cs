using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyMoviment : FuzzyOperations
{
    public IaCarro Carro;
    //Declare Fuzzy Membership Curves
    public AnimationCurve Velocidade_Baixa;
    public AnimationCurve Velocidade_Media;
    public AnimationCurve Velocidade_Alta;

    public AnimationCurve Distancia_Baixa;
    public AnimationCurve Distancia_Media;
    public AnimationCurve Distancia_Alta;

    public AnimationCurve Aceleracao_Baixissima;
    public AnimationCurve Aceleracao_Baixa;
    public AnimationCurve Aceleracao_Media;
    public AnimationCurve Aceleracao_Alta;
    public AnimationCurve Aceleracao_Altissima;

    //Tables to store the membership of a given value to each fuzzy set
    private Dictionary<string, float> Dicionario_Velocidade;
    private Dictionary<string, float> Dicionario_Distancia;
    private Dictionary<string, float> Dicionario_Aceleracao;

    //Table with the Fuzzy Rules to be Evaluated
    public Dictionary<string, string> Regras;

    //Defuzzified output
    private float aceleracaoFinal;

    public float maxVelocidade;
    public float maxDistancia;
    private float maxAceleracao;


    public void FuzzifyVelocidade(float inputValue)
    {
        Dicionario_Velocidade["Baixa"] = Velocidade_Baixa.Evaluate(inputValue);
        Dicionario_Velocidade["Media"] = Velocidade_Media.Evaluate(inputValue);
        Dicionario_Velocidade["Alta"] = Velocidade_Alta.Evaluate(inputValue);
    }

    public void FuzzifyDistancia(float inputValue)
    {
        Dicionario_Distancia["Baixa"] = Distancia_Baixa.Evaluate(inputValue);
        Dicionario_Distancia["Media"] = Distancia_Media.Evaluate(inputValue);
        Dicionario_Distancia["Alta"] = Distancia_Alta.Evaluate(inputValue);
    }

    public void SetAllValues()
    {
        // Set values for each membership function
        SetFuzzify(new[] { new Vector2(0f, 1f), new Vector2(maxVelocidade/2, 0f) }, Velocidade_Baixa);
        SetFuzzify(new[] { new Vector2(0f, 0f), new Vector2(maxVelocidade/2, 1f), new Vector2(maxVelocidade, 0) }, Velocidade_Media);
        SetFuzzify(new[] { new Vector2(maxVelocidade/2, 0f), new Vector2(maxVelocidade, 1f) }, Velocidade_Alta);

        SetFuzzify(new[] { new Vector2(0f, 1f), new Vector2(maxDistancia/2, 0f) }, Distancia_Baixa);
        SetFuzzify(new[] { new Vector2(0f, 0f), new Vector2(maxDistancia/2, 1f), new Vector2(maxDistancia, 0) }, Distancia_Media);
        SetFuzzify(new[] { new Vector2(maxDistancia/2, 0f), new Vector2(maxDistancia, 1f) }, Distancia_Alta);

        SetFuzzify(new[] { new Vector2((-2 * maxAceleracao / 4), 0f), new Vector2((-1 * maxAceleracao / 4), 1f), new Vector2(0, 0f) }, Aceleracao_Baixissima);
        SetFuzzify(new[] { new Vector2((-1 * maxAceleracao / 4), 0f), new Vector2(0, 1f), new Vector2((maxAceleracao / 4), 0f) }, Aceleracao_Baixa);
        SetFuzzify(new[] { new Vector2(0f, 0f), new Vector2((maxAceleracao / 4), 1f), new Vector2((2 * maxAceleracao / 4), 0) }, Aceleracao_Media);
        SetFuzzify(new[] { new Vector2((maxAceleracao / 4), 0f), new Vector2((2 * maxAceleracao / 4), 1f), new Vector2((3 * maxAceleracao / 4), 0f) }, Aceleracao_Alta);
        SetFuzzify(new[] { new Vector2((2 * maxAceleracao / 4), 0f), new Vector2((3 * maxAceleracao / 4), 1f), new Vector2((4 * maxAceleracao / 4), 0f) }, Aceleracao_Altissima);

    }

    public void SetAllRules()
    {
        //Initialize RuleBase dictionary and fill in the rules (distancia + velocidade)
        Regras = new Dictionary<string, string>();

        Regras.Add("AltaAlta", "Altissima");
        Regras.Add("AltaMedia", "Alta");
        Regras.Add("AltaBaixa", "Media");

        Regras.Add("MediaAlta", "Baixissima");
        Regras.Add("MediaMedia", "Baixa");
        Regras.Add("MediaBaixa", "Media");

        Regras.Add("BaixaAlta", "Baixissima");
        Regras.Add("BaixaMedia", "Baixissima");
        Regras.Add("BaixaBaixa", "Baixissima");
    }

    public void InitializeValues()
    {
        //Intialize membership values
        Dicionario_Velocidade = new Dictionary<string, float>();
        Dicionario_Distancia = new Dictionary<string, float>();
        Dicionario_Aceleracao = new Dictionary<string, float>();

        Dicionario_Velocidade.Add("Baixa", 0f);
        Dicionario_Velocidade.Add("Media", 0f);
        Dicionario_Velocidade.Add("Alta", 0f);

        Dicionario_Distancia.Add("Baixa", 0f);
        Dicionario_Distancia.Add("Media", 0f);
        Dicionario_Distancia.Add("Alta", 0f);

        Dicionario_Aceleracao.Add("Baixissima", 0f);
        Dicionario_Aceleracao.Add("Baixa", 0f);
        Dicionario_Aceleracao.Add("Media", 0f);
        Dicionario_Aceleracao.Add("Alta", 0f);
        Dicionario_Aceleracao.Add("Altissima", 0f);

    }

    //Clean any trash membership values that might be left from a previous operation
    public void ResetValues()
    {
        Dicionario_Velocidade["Baixa"] = 0f;
        Dicionario_Velocidade["Media"] = 0f;
        Dicionario_Velocidade["Alta"] = 0f;

        Dicionario_Distancia["Baixa"] = 0f;
        Dicionario_Distancia["Media"] = 0f;
        Dicionario_Distancia["Alta"] = 0f;

        Dicionario_Aceleracao["Baixissima"] = 0f;
        Dicionario_Aceleracao["Baixa"] = 0f;
        Dicionario_Aceleracao["Media"] = 0f;
        Dicionario_Aceleracao["Alta"] = 0f;
        Dicionario_Aceleracao["Altissima"] = 0f;

    }

    //Iterate over all possible memberships and process the rules present
    public void EvaluateRuleBase()
    {
        foreach (var keyA in Dicionario_Velocidade.Keys)
        {
            foreach (var keyF in Dicionario_Distancia.Keys)
            {
                if (Dicionario_Distancia[keyF] > 0 && Dicionario_Velocidade[keyA] > 0)
                {
                    string ResultadoAceleracao = Regras[keyF + keyA];
                    Dicionario_Aceleracao[ResultadoAceleracao] = Mathf.Max(Mathf.Min(Dicionario_Velocidade[keyA], Dicionario_Distancia[keyF]), Dicionario_Aceleracao[ResultadoAceleracao]);
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
            if (keyP == "Altissima")
            {
                area = CalculateTrapezoidArea(Aceleracao_Altissima, U_a);
                center = CalculateCenter(Aceleracao_Altissima, U_a);
            }
            else if (keyP == "Alta")
            {
                area = CalculateTrapezoidArea(Aceleracao_Alta, U_a);
                center = CalculateCenter(Aceleracao_Alta, U_a);
            }
            else if (keyP == "Media")
            {
                area = CalculateTrapezoidArea(Aceleracao_Media, U_a);
                center = CalculateCenter(Aceleracao_Media, U_a);
            }
            else if (keyP == "Baixa")
            {
                area = CalculateTrapezoidArea(Aceleracao_Baixa, U_a);
                center = CalculateCenter(Aceleracao_Baixa, U_a);
            }
            else
            {
                area = CalculateTrapezoidArea(Aceleracao_Baixissima, U_a);
                center = CalculateCenter(Aceleracao_Baixissima, U_a);
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

    private void OnEnable()
    {
        maxAceleracao = maxVelocidade * 7.5f;
        Carro.SetDistanciaFrenteCol(maxDistancia);
        Carro = GetComponent<IaCarro>();
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
        RotateCar();
    }

    public void RotateCar()
    {
        ResetValues();

        FuzzifyVelocidade(Carro.GetVelocidade());
        FuzzifyDistancia(Carro.GetDistanciaFrenteCol());

        EvaluateRuleBase();
        Carro.SetAceleracao(DefuzzifyAceleracao());
    }

}
