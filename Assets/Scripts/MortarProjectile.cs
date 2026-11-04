using System.Collections.Generic;
using UnityEngine;

public class MortarProjectile : MonoBehaviour
{
    public ParticleSystem impactPrefab;
    private Vector3 origin, destination;
    private float elapsed, duration, height, radius, damage;
    private Tower owner;
    private ElementType element;
    private bool initialized, exploded;
    private bool reusable;
    private ParticleSystem effect;
    private Renderer[] shellRenderers;
    private MaterialPropertyBlock properties;
    private Collider[] overlaps = new Collider[64];
    private readonly HashSet<Enemy> hit = new HashSet<Enemy>();

    private void Awake()
    {
        shellRenderers = GetComponentsInChildren<Renderer>();
        properties = new MaterialPropertyBlock();
        // Efekt merminin çocuğudur; kule silinince havuzla birlikte temizlenir.
        if (impactPrefab != null)
        {
            effect = Instantiate(impactPrefab, transform);
            var main = effect.main;
            main.stopAction = ParticleSystemStopAction.None;
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void PrepareForReuse()
    {
        reusable = true;
        gameObject.SetActive(false);
    }

    public void Initialize(Vector3 impactPoint, float attackDamage, float criticalChance, Tower source)
    {
        if (initialized || source == null || source.data == null) return;
        initialized = true;
        exploded = false;
        elapsed = 0f;
        hit.Clear();
        foreach (var renderer in shellRenderers) renderer.enabled = true;
        origin = transform.position;
        destination = impactPoint;
        owner = source;
        element = source.SelectedElement;
        duration = Mathf.Max(0.1f, source.data.mortarFlightTime);
        height = Mathf.Max(0f, source.data.mortarArcHeight);
        radius = Mathf.Max(0.1f, source.data.explosionRadius);
        // Kritik tek atış için belirlenir. Element çarpanını her Enemy kendisi uygular.
        damage = attackDamage * (Random.value < Mathf.Clamp01(criticalChance)
            ? Mathf.Max(1f, source.data.mortarCriticalMultiplier) : 1f);
        Color tint = element == ElementType.Water ? new Color(.2f, .65f, 1f)
            : element == ElementType.Fire ? new Color(1f, .3f, .08f)
            : element == ElementType.Earth ? new Color(.65f, .43f, .2f)
            : element == ElementType.Air ? new Color(.65f, .95f, .9f) : new Color(.25f, .22f, .16f);
        properties.SetColor("_Color", tint);
        properties.SetColor("_BaseColor", tint);
        foreach (var renderer in shellRenderers)
        {
            renderer.SetPropertyBlock(properties);
        }
    }

    private void Update()
    {
        if (!initialized) return;
        if (exploded)
        {
            if (effect == null || !effect.IsAlive(true)) Release();
            return;
        }
        elapsed += Time.deltaTime; // Duraklatma ve oyun hızı ile birlikte ilerler.
        float progress = Mathf.Clamp01(elapsed / duration);
        transform.position = Vector3.Lerp(origin, destination, progress)
            + Vector3.up * (4f * height * progress * (1f - progress));
        if (progress >= 1f) Explode();
    }

    private void Explode()
    {
        if (exploded || !initialized) return;
        exploded = true;
        transform.position = destination;
        foreach (var renderer in shellRenderers) renderer.enabled = false;
        if (effect != null)
        {
            effect.transform.localPosition = Vector3.zero;
            float scale = Mathf.Max(.0001f, Mathf.Abs(transform.lossyScale.x));
            effect.transform.localScale = Vector3.one * (radius / scale);
            effect.Play();
        }
        int count;
        // Tam dolu tamponu büyütüp yeniden sorgula: yoğun gruplarda hedef kaybetme.
        while ((count = Physics.OverlapSphereNonAlloc(destination, radius, overlaps, ~0, QueryTriggerInteraction.Collide)) == overlaps.Length)
            System.Array.Resize(ref overlaps, overlaps.Length * 2);
        for (int i = 0; i < count; i++)
        {
            var collider = overlaps[i];
            overlaps[i] = null;
            var enemy = collider.GetComponentInParent<Enemy>();
            if (enemy == null || enemy.enemyData == null || enemy.IsAir
                || !enemy.gameObject.activeInHierarchy || enemy.CurrentHealth <= 0f || !hit.Add(enemy)) continue;
            // Aynı düşmanın birden çok collider'ı olsa da yalnız bir kez hasar.
            enemy.TakeDamage(damage, element, owner);
        }
        if (effect == null) Release();
    }

    private void Release()
    {
        initialized = false;
        owner = null;
        hit.Clear();
        if (reusable) gameObject.SetActive(false);
        else Destroy(gameObject);
    }
}
