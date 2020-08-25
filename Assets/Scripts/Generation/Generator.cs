using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Generator
{
    private Field m_Field;
    private int m_SimulationSpeed;
    private List<MovingAgent> m_Animals = new List<MovingAgent>();
    private Dictionary<int, TeamObject> m_Food = new Dictionary<int, TeamObject>();

    public Generator()
    {
        EventManager.Instance.AddListener(EventType.AgentGotTargetCell, OnEvent);
        EventManager.Instance.AddListener(EventType.SimulationSceneLoaded, OnEvent);
        
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }
    
    private void GenerateField(FieldData fieldData)
    {
        m_SimulationSpeed = fieldData.Speed;
        var fieldCellPrefab = Resources.Load<GameObject>("FieldCell");
        m_Field = new GameObject("Field").AddComponent<Field>();
        m_Field.Initialize(fieldData.Size);

        // Инстанциирование клеток поля
        for (var i = 0; i < fieldData.Size; i++)
        {
            for (var j = 0; j < fieldData.Size; j++)
            {
                var fieldCell = Object.Instantiate(fieldCellPrefab, new Vector3(j, 0, i), Quaternion.identity,
                    m_Field.transform).GetComponent<FieldCell>();
                fieldCell.gameObject.name = "[" + i + "," + j + "]";
                m_Field.FieldCells[i][j] = fieldCell;
                m_Field.FreeCells.Add(fieldCell);

                // Установка цвета клеткам поля
                if (!fieldCell.gameObject.TryGetComponent<ColoredObject>(out var coloredCell)) continue;
                if (i % 2 == 0)
                {
                    coloredCell.Initialise(j % 2 == 0 ? Color.white : Color.black);
                }
                else
                {
                    coloredCell.Initialise(j % 2 == 0 ? Color.black : Color.white);
                }
            }
        }
    }

    private void GenerateAnimals(FieldData fieldData)
    {
        var animalPrefab = Resources.Load<GameObject>("AnimalBase");
        var animals = new GameObject("Animals");
        MovingAgent.Speed = m_SimulationSpeed;

        for (var i = 0; i < fieldData.AnimalsAmount; i++)
        {
            var cell = m_Field.GetRandomFreeCell();
            cell.AnimalOnCell = Object.Instantiate(animalPrefab, cell.transform.position + Vector3.up,
                Quaternion.identity,
                animals.transform).GetComponent<TeamObject>();
            var animal = cell.AnimalOnCell;
            animal.Initialise();
            var agent = animal.gameObject.GetComponent<MovingAgent>();
            agent.CurrentCell = cell;
            var targetCell = GenerateFoodForAnimal(animal);
            if (targetCell != null)
                agent.TargetCell = targetCell;
            else
            {
                Debug.Log("При генерации еды возникла ошибка");
            }

            m_Animals.Add(agent);
        }
    }

    private FieldCell GenerateFoodForAnimal(TeamObject animal, bool isInstantiationNeeded = true)
    {
        if (animal.gameObject.TryGetComponent<MovingAgent>(out var agent))
        {
            var coordinates = agent.CurrentCell.Coordinates;
            var reachableCellsCount = m_SimulationSpeed * 5;

            //Получаем координаты для генерации еды с учётом максимального достижимого расстояния и размерности поля
            Vector2Int foodCoordinates;
            do
            {
                var newX = Random.Range(coordinates.x - reachableCellsCount, coordinates.x + reachableCellsCount);
                var newY = Random.Range(coordinates.y - reachableCellsCount, coordinates.y + reachableCellsCount);

                newX = newX < 0 ? 0 : newX;
                newY = newY < 0 ? 0 : newY;
                newX %= m_Field.FieldCells.Length;
                newY %= m_Field.FieldCells.Length;

                foodCoordinates = new Vector2Int(newY, newX);
            } while (foodCoordinates.Equals(coordinates));

            GameObject foodObject;
            if (isInstantiationNeeded)
            {
                return InstantiateFood(foodCoordinates, out foodObject, animal.Id);
            }

            foodObject = m_Food[animal.Id].gameObject;
            foodObject.transform.position = new Vector3(foodCoordinates.x, 0, foodCoordinates.y) + Vector3.up;
            foodObject.SetActive(true);
            return m_Field.FieldCells[foodCoordinates.y][foodCoordinates.x];
        }

        Debug.LogError("Отсутсвует компонент MovingAgent на префабе AnimalBase");
        return null;
    }

    private FieldCell InstantiateFood(Vector2Int foodCoordinates, out GameObject foodObject, int id)
    {
        var foodPrefab = Resources.Load<GameObject>("FoodObject");
        foodObject = Object.Instantiate(foodPrefab,
            new Vector3(foodCoordinates.x, 0, foodCoordinates.y) + Vector3.up, Quaternion.identity);
        if (foodObject.TryGetComponent<TeamObject>(out var foodByTeam))
        {
            if (TeamObject.Teams.ContainsKey(id))
            {
                foodByTeam.Initialise(TeamObject.Teams[id]);
                m_Food.Add(id, foodByTeam);
                return m_Field.FieldCells[foodCoordinates.y][foodCoordinates.x];
            }

            Object.Destroy(foodObject);
            Debug.LogError("ID животного не содержится в словаре Teams");
            return null;
        }

        Object.Destroy(foodObject);
        Debug.LogError("Отсутсвует компонент TeamObject на префабе FoodObject");
        return null;
    }

    private void OnEvent(EventType eventType, Component sender, object param)
    {
        switch (eventType)
        {
            case EventType.SimulationSceneLoaded:
                var fieldData = (FieldData) param;
                if (GameManager.Instance.PlayerData.FieldData.Animals == null)
                {
                    GenerateField(fieldData);
                    GenerateAnimals(fieldData);
                }
                else
                {
                    GenerateField(fieldData);
                    LoadObjects();
                }

                break;
            case EventType.AgentGotTargetCell:
                var animal = sender as TeamObject;
                if (animal != null)
                {
                    var targetCell = GenerateFoodForAnimal(animal, false);
                    if (targetCell != null)
                        animal.gameObject.GetComponent<MovingAgent>().TargetCell = targetCell;
                }

                break;
        }
    }
    private void LoadObjects()
    {
        var animalsData = GameManager.Instance.PlayerData.FieldData.Animals;

        MovingAgent.Speed = m_SimulationSpeed;
        var animalPrefab = Resources.Load<GameObject>("AnimalBase");
        
        foreach (var animalData in animalsData)
        {
            var animals = new GameObject("Animals");
            var id = animalData.Color.GetHashCode();
            TeamObject.Teams.Add(id, animalData.Color);

            var cell = m_Field.FieldCells[animalData.CurrentCell.x][animalData.CurrentCell.y];

            cell.AnimalOnCell = Object.Instantiate(animalPrefab, cell.transform.position + Vector3.up,
                Quaternion.identity,
                animals.transform).GetComponent<TeamObject>();
            var animal = cell.AnimalOnCell;
            animal.Initialise(animalData.Color);
            var agent = animal.gameObject.GetComponent<MovingAgent>();
            agent.CurrentCell = cell;
            agent.TargetCell = InstantiateFood(animalData.TargetCell, out var foodObject, id);
        }
    }
    public AnimalData[] CreatAnimalData()
    {
        var animals = new List<AnimalData>();

        foreach (var fieldRow in m_Field.FieldCells)
        {
            foreach (var animalOnCell in fieldRow.Where(cell => cell.AnimalOnCell != null)
                .Select(cell => cell.AnimalOnCell))
            {
                var agent = animalOnCell.gameObject.GetComponent<MovingAgent>();
                animals.Add(new AnimalData
                {
                    Color = TeamObject.Teams[animalOnCell.Id],
                    CurrentCell = agent.CurrentCell.Coordinates,
                    TargetCell = agent.TargetCell.Coordinates
                });
            }
        }

        return animals.ToArray();
    }
    
    private void OnActiveSceneChanged(Scene scene1, Scene scene2)
    {
        if (scene2.buildIndex != 0) return;
        m_Animals.Clear();
        m_Food.Clear();
        TeamObject.Teams.Clear();
    }
}