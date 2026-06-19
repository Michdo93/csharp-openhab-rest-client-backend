using Microsoft.AspNetCore.Mvc;
using OpenHABRestClient;
using OpenHABBackend.Models;

namespace OpenHABBackend.Controllers;

public abstract class BaseController(ClientFactory factory) : ControllerBase
{
    protected ClientFactory Factory = factory;
    protected IActionResult Err(Exception e) =>
        StatusCode(502, new { error = e.Message });
}

// ── System ────────────────────────────────────────────────────────────────────
[ApiController] [Route("api")]
public class SystemController(ClientFactory f) : BaseController(f)
{
    [HttpPost("uuid")]           public IActionResult GetUUID([FromBody] OpenHABRequest r)      { try { return Ok(new UUID(Factory.Build(r)).GetUUID()); } catch (Exception e) { return Err(e); } }
    [HttpPost("systeminfo")]     public IActionResult GetSystemInfo([FromBody] OpenHABRequest r) { try { return Ok(new Systeminfo(Factory.Build(r)).GetSystemInfo()); } catch (Exception e) { return Err(e); } }
    [HttpPost("systeminfo/uom")] public IActionResult GetUoMInfo([FromBody] OpenHABRequest r)   { try { return Ok(new Systeminfo(Factory.Build(r)).GetUoMInfo()); } catch (Exception e) { return Err(e); } }
}

// ── Items ─────────────────────────────────────────────────────────────────────
[ApiController] [Route("api/items")]
public class ItemsController(ClientFactory f) : BaseController(f)
{
    [HttpPost]                                   public IActionResult GetItems([FromBody] OpenHABRequest r)                                                              { try { return Ok(new Items(Factory.Build(r)).GetItems()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}")]                         public IActionResult GetItem([FromRoute] string name, [FromBody] OpenHABRequest r)                                      { try { return Ok(new Items(Factory.Build(r)).GetItem(name)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/state")]                   public IActionResult GetState([FromRoute] string name, [FromBody] OpenHABRequest r)                                     { try { return Ok(new Items(Factory.Build(r)).GetItemState(name)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/state/update")]            public IActionResult UpdateState([FromRoute] string name, [FromBody] OpenHABRequest r)                                  { try { return Ok(new Items(Factory.Build(r)).UpdateItemState(name, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/command")]                 public IActionResult SendCommand([FromRoute] string name, [FromBody] OpenHABRequest r)                                  { try { return Ok(new Items(Factory.Build(r)).SendCommand(name, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/addOrUpdate")]             public IActionResult AddOrUpdate([FromRoute] string name, [FromBody] OpenHABRequest r)                                  { try { return Ok(new Items(Factory.Build(r)).AddOrUpdateItem(name, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/delete")]                  public IActionResult Delete([FromRoute] string name, [FromBody] OpenHABRequest r)                                       { try { return Ok(new Items(Factory.Build(r)).DeleteItem(name)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/members/{member}/add")]    public IActionResult AddMember([FromRoute] string name, [FromRoute] string member, [FromBody] OpenHABRequest r)         { try { return Ok(new Items(Factory.Build(r)).AddGroupMember(name, member)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/members/{member}/remove")] public IActionResult RemoveMember([FromRoute] string name, [FromRoute] string member, [FromBody] OpenHABRequest r)      { try { return Ok(new Items(Factory.Build(r)).RemoveGroupMember(name, member)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/tags/{tag}/add")]          public IActionResult AddTag([FromRoute] string name, [FromRoute] string tag, [FromBody] OpenHABRequest r)               { try { return Ok(new Items(Factory.Build(r)).AddTag(name, tag)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/tags/{tag}/remove")]       public IActionResult RemoveTag([FromRoute] string name, [FromRoute] string tag, [FromBody] OpenHABRequest r)            { try { return Ok(new Items(Factory.Build(r)).RemoveTag(name, tag)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/metadata/{ns}/add")]       public IActionResult AddMeta([FromRoute] string name, [FromRoute] string ns, [FromBody] OpenHABRequest r)               { try { return Ok(new Items(Factory.Build(r)).AddMetadata(name, ns, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/metadata/{ns}/remove")]    public IActionResult RemoveMeta([FromRoute] string name, [FromRoute] string ns, [FromBody] OpenHABRequest r)            { try { return Ok(new Items(Factory.Build(r)).RemoveMetadata(name, ns)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/metadata/namespaces")]     public IActionResult GetNS([FromRoute] string name, [FromBody] OpenHABRequest r)                                        { try { return Ok(new Items(Factory.Build(r)).GetMetadataNamespaces(name)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/semantic/{cls}")]          public IActionResult GetSemantic([FromRoute] string name, [FromRoute] string cls, [FromBody] OpenHABRequest r)          { try { return Ok(new Items(Factory.Build(r)).GetSemanticItem(name, cls)); } catch (Exception e) { return Err(e); } }
    [HttpPost("metadata/purge")]                 public IActionResult Purge([FromBody] OpenHABRequest r)                                                                 { try { return Ok(new Items(Factory.Build(r)).PurgeOrphanedMetadata()); } catch (Exception e) { return Err(e); } }
}

// ── Things ────────────────────────────────────────────────────────────────────
[ApiController] [Route("api/things")]
public class ThingsController(ClientFactory f) : BaseController(f)
{
    [HttpPost]                          public IActionResult Get([FromBody] OpenHABRequest r)                                          { try { return Ok(new Things(Factory.Build(r)).GetThings()); } catch (Exception e) { return Err(e); } }
    [HttpPost("create")]                public IActionResult Create([FromBody] OpenHABRequest r)                                       { try { return Ok(new Things(Factory.Build(r)).CreateThing(r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}")]                 public IActionResult GetOne([FromRoute] string uid, [FromBody] OpenHABRequest r)               { try { return Ok(new Things(Factory.Build(r)).GetThing(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/update")]          public IActionResult Update([FromRoute] string uid, [FromBody] OpenHABRequest r)               { try { return Ok(new Things(Factory.Build(r)).UpdateThing(uid, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/delete")]          public IActionResult Delete([FromRoute] string uid, [FromBody] OpenHABRequest r)               { try { return Ok(new Things(Factory.Build(r)).DeleteThing(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/config")]          public IActionResult Config([FromRoute] string uid, [FromBody] OpenHABRequest r)               { try { return Ok(new Things(Factory.Build(r)).UpdateThingConfiguration(uid, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/config/status")]   public IActionResult ConfigStatus([FromRoute] string uid, [FromBody] OpenHABRequest r)         { try { return Ok(new Things(Factory.Build(r)).GetThingConfigStatus(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/enable")]          public IActionResult Enable([FromRoute] string uid, [FromBody] OpenHABRequest r)               { try { return Ok(new Things(Factory.Build(r)).EnableThing(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/disable")]         public IActionResult Disable([FromRoute] string uid, [FromBody] OpenHABRequest r)              { try { return Ok(new Things(Factory.Build(r)).DisableThing(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/status")]          public IActionResult Status([FromRoute] string uid, [FromBody] OpenHABRequest r)               { try { return Ok(new Things(Factory.Build(r)).GetThingStatus(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/firmware/status")] public IActionResult FwStatus([FromRoute] string uid, [FromBody] OpenHABRequest r)            { try { return Ok(new Things(Factory.Build(r)).GetThingFirmwareStatus(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/firmwares")]       public IActionResult Firmwares([FromRoute] string uid, [FromBody] OpenHABRequest r)            { try { return Ok(new Things(Factory.Build(r)).GetThingFirmwares(uid)); } catch (Exception e) { return Err(e); } }
}

// ── Rules ─────────────────────────────────────────────────────────────────────
[ApiController] [Route("api/rules")]
public class RulesController(ClientFactory f) : BaseController(f)
{
    [HttpPost]                       public IActionResult Get([FromBody] OpenHABRequest r)                                      { try { return Ok(new Rules(Factory.Build(r)).GetRules()); } catch (Exception e) { return Err(e); } }
    [HttpPost("create")]             public IActionResult Create([FromBody] OpenHABRequest r)                                   { try { return Ok(new Rules(Factory.Build(r)).CreateRule(r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}")]              public IActionResult GetOne([FromRoute] string uid, [FromBody] OpenHABRequest r)           { try { return Ok(new Rules(Factory.Build(r)).GetRule(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/update")]       public IActionResult Update([FromRoute] string uid, [FromBody] OpenHABRequest r)           { try { return Ok(new Rules(Factory.Build(r)).UpdateRule(uid, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/delete")]       public IActionResult Delete([FromRoute] string uid, [FromBody] OpenHABRequest r)           { try { return Ok(new Rules(Factory.Build(r)).DeleteRule(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/enable")]       public IActionResult Enable([FromRoute] string uid, [FromBody] OpenHABRequest r)           { try { return Ok(new Rules(Factory.Build(r)).Enable(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/disable")]      public IActionResult Disable([FromRoute] string uid, [FromBody] OpenHABRequest r)          { try { return Ok(new Rules(Factory.Build(r)).Disable(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/runnow")]       public IActionResult RunNow([FromRoute] string uid, [FromBody] OpenHABRequest r)           { try { return Ok(new Rules(Factory.Build(r)).RunNow(uid, r.Body)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/config")]       public IActionResult GetConfig([FromRoute] string uid, [FromBody] OpenHABRequest r)        { try { return Ok(new Rules(Factory.Build(r)).GetConfiguration(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/config/update")]public IActionResult UpdateConfig([FromRoute] string uid, [FromBody] OpenHABRequest r)    { try { return Ok(new Rules(Factory.Build(r)).UpdateConfiguration(uid, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/actions")]      public IActionResult Actions([FromRoute] string uid, [FromBody] OpenHABRequest r)          { try { return Ok(new Rules(Factory.Build(r)).GetActions(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/conditions")]   public IActionResult Conditions([FromRoute] string uid, [FromBody] OpenHABRequest r)       { try { return Ok(new Rules(Factory.Build(r)).GetConditions(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/triggers")]     public IActionResult Triggers([FromRoute] string uid, [FromBody] OpenHABRequest r)         { try { return Ok(new Rules(Factory.Build(r)).GetTriggers(uid)); } catch (Exception e) { return Err(e); } }
}

// ── Addons ────────────────────────────────────────────────────────────────────
[ApiController] [Route("api/addons")]
public class AddonsController(ClientFactory f) : BaseController(f)
{
    [HttpPost]                   public IActionResult Get([FromBody] OpenHABRequest r)                                { try { return Ok(new Addons(Factory.Build(r)).GetAddons()); } catch (Exception e) { return Err(e); } }
    [HttpPost("types")]          public IActionResult Types([FromBody] OpenHABRequest r)                              { try { return Ok(new Addons(Factory.Build(r)).GetAddonTypes()); } catch (Exception e) { return Err(e); } }
    [HttpPost("suggestions")]    public IActionResult Suggestions([FromBody] OpenHABRequest r)                        { try { return Ok(new Addons(Factory.Build(r)).GetAddonSuggestions()); } catch (Exception e) { return Err(e); } }
    [HttpPost("services")]       public IActionResult Services([FromBody] OpenHABRequest r)                           { try { return Ok(new Addons(Factory.Build(r)).GetAddonServices()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{id}")]           public IActionResult GetOne([FromRoute] string id, [FromBody] OpenHABRequest r)      { try { return Ok(new Addons(Factory.Build(r)).GetAddon(id)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{id}/install")]   public IActionResult Install([FromRoute] string id, [FromBody] OpenHABRequest r)     { try { return Ok(new Addons(Factory.Build(r)).InstallAddon(id)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{id}/uninstall")] public IActionResult Uninstall([FromRoute] string id, [FromBody] OpenHABRequest r)  { try { return Ok(new Addons(Factory.Build(r)).UninstallAddon(id)); } catch (Exception e) { return Err(e); } }
}

// ── Audio ─────────────────────────────────────────────────────────────────────
[ApiController] [Route("api/audio")]
public class AudioController(ClientFactory f) : BaseController(f)
{
    [HttpPost("defaultsink")]   public IActionResult DefaultSink([FromBody] OpenHABRequest r)   { try { return Ok(new Audio(Factory.Build(r)).GetDefaultSink()); } catch (Exception e) { return Err(e); } }
    [HttpPost("defaultsource")] public IActionResult DefaultSource([FromBody] OpenHABRequest r) { try { return Ok(new Audio(Factory.Build(r)).GetDefaultSource()); } catch (Exception e) { return Err(e); } }
    [HttpPost("sinks")]         public IActionResult Sinks([FromBody] OpenHABRequest r)         { try { return Ok(new Audio(Factory.Build(r)).GetSinks()); } catch (Exception e) { return Err(e); } }
    [HttpPost("sources")]       public IActionResult Sources([FromBody] OpenHABRequest r)       { try { return Ok(new Audio(Factory.Build(r)).GetSources()); } catch (Exception e) { return Err(e); } }
}

// ── Logging ───────────────────────────────────────────────────────────────────
[ApiController] [Route("api/logging")]
public class LoggingController(ClientFactory f) : BaseController(f)
{
    [HttpPost]               public IActionResult Get([FromBody] OpenHABRequest r)                                                { try { return Ok(new Logging(Factory.Build(r)).GetLoggers()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}")]     public IActionResult GetOne([FromRoute] string name, [FromBody] OpenHABRequest r)                    { try { return Ok(new Logging(Factory.Build(r)).GetLogger(name)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/set")] public IActionResult Set([FromRoute] string name, [FromBody] OpenHABRequest r)                      { try { var level = r.Params?.GetValueOrDefault("level") ?? "INFO"; return Ok(new Logging(Factory.Build(r)).ModifyOrAddLogger(name, level)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/delete")]public IActionResult Del([FromRoute] string name, [FromBody] OpenHABRequest r)                    { try { return Ok(new Logging(Factory.Build(r)).RemoveLogger(name)); } catch (Exception e) { return Err(e); } }
}

// ── Links ─────────────────────────────────────────────────────────────────────
[ApiController] [Route("api/links")]
public class LinksController(ClientFactory f) : BaseController(f)
{
    [HttpPost]                              public IActionResult Get([FromBody] OpenHABRequest r)                                                          { try { return Ok(new Links(Factory.Build(r)).GetLinks()); } catch (Exception e) { return Err(e); } }
    [HttpPost("orphan")]                    public IActionResult Orphan([FromBody] OpenHABRequest r)                                                       { try { return Ok(new Links(Factory.Build(r)).GetOrphanLinks()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{item}/{channel}/link")]     public IActionResult Link([FromRoute] string item, [FromRoute] string channel, [FromBody] OpenHABRequest r)    { try { return Ok(new Links(Factory.Build(r)).LinkItemToChannel(item, channel, r.Body ?? "{}")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{item}/{channel}/unlink")]   public IActionResult Unlink([FromRoute] string item, [FromRoute] string channel, [FromBody] OpenHABRequest r)  { try { return Ok(new Links(Factory.Build(r)).UnlinkItemFromChannel(item, channel)); } catch (Exception e) { return Err(e); } }
}

// ── ChannelTypes / ThingTypes / ConfigDescriptions ───────────────────────────
[ApiController] [Route("api/channel-types")]
public class ChannelTypesController(ClientFactory f) : BaseController(f)
{
    [HttpPost]       public IActionResult Get([FromBody] OpenHABRequest r)                                 { try { return Ok(new ChannelTypes(Factory.Build(r)).GetChannelTypes()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}")] public IActionResult GetOne([FromRoute] string uid, [FromBody] OpenHABRequest r)   { try { return Ok(new ChannelTypes(Factory.Build(r)).GetChannelType(uid)); } catch (Exception e) { return Err(e); } }
}

[ApiController] [Route("api/thing-types")]
public class ThingTypesController(ClientFactory f) : BaseController(f)
{
    [HttpPost]       public IActionResult Get([FromBody] OpenHABRequest r)                                 { try { return Ok(new ThingTypes(Factory.Build(r)).GetThingTypes()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}")] public IActionResult GetOne([FromRoute] string uid, [FromBody] OpenHABRequest r)   { try { return Ok(new ThingTypes(Factory.Build(r)).GetThingType(uid)); } catch (Exception e) { return Err(e); } }
}

[ApiController] [Route("api/config-descriptions")]
public class ConfigDescController(ClientFactory f) : BaseController(f)
{
    [HttpPost] public IActionResult Get([FromBody] OpenHABRequest r) { try { return Ok(new ConfigDescriptions(Factory.Build(r)).GetConfigDescriptions()); } catch (Exception e) { return Err(e); } }
}

// ── Persistence ───────────────────────────────────────────────────────────────
[ApiController] [Route("api/persistence")]
public class PersistenceController(ClientFactory f) : BaseController(f)
{
    [HttpPost]                 public IActionResult Get([FromBody] OpenHABRequest r)                                  { try { return Ok(new Persistence(Factory.Build(r)).GetServices()); } catch (Exception e) { return Err(e); } }
    [HttpPost("items")]        public IActionResult Items([FromBody] OpenHABRequest r)                                { try { return Ok(new Persistence(Factory.Build(r)).GetItemsFromService()); } catch (Exception e) { return Err(e); } }
    [HttpPost("items/{item}")] public IActionResult ItemData([FromRoute] string item, [FromBody] OpenHABRequest r)    { try { var svcId = r.Params?.GetValueOrDefault("serviceId") ?? ""; return Ok(new Persistence(Factory.Build(r)).GetItemPersistenceData(item, svcId)); } catch (Exception e) { return Err(e); } }
}

// ── Discovery + Inbox ─────────────────────────────────────────────────────────
[ApiController] [Route("api/discovery")]
public class DiscoveryController(ClientFactory f) : BaseController(f)
{
    [HttpPost]              public IActionResult Get([FromBody] OpenHABRequest r)                              { try { return Ok(new Discovery(Factory.Build(r)).GetDiscoveryBindings()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{bid}/scan")]public IActionResult Scan([FromRoute] string bid, [FromBody] OpenHABRequest r)     { try { return Ok(new Discovery(Factory.Build(r)).StartBindingScan(bid)); } catch (Exception e) { return Err(e); } }
}

[ApiController] [Route("api/inbox")]
public class InboxController(ClientFactory f) : BaseController(f)
{
    [HttpPost]                   public IActionResult Get([FromBody] OpenHABRequest r)                                { try { return Ok(new Inbox(Factory.Build(r)).GetDiscoveredThings()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/approve")]  public IActionResult Approve([FromRoute] string uid, [FromBody] OpenHABRequest r)   { try { return Ok(new Inbox(Factory.Build(r)).ApproveDiscoveryResult(uid, r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/ignore")]   public IActionResult Ignore([FromRoute] string uid, [FromBody] OpenHABRequest r)    { try { return Ok(new Inbox(Factory.Build(r)).IgnoreDiscoveryResult(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/unignore")] public IActionResult Unignore([FromRoute] string uid, [FromBody] OpenHABRequest r)  { try { return Ok(new Inbox(Factory.Build(r)).UnignoreDiscoveryResult(uid)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}/delete")]   public IActionResult Delete([FromRoute] string uid, [FromBody] OpenHABRequest r)    { try { return Ok(new Inbox(Factory.Build(r)).RemoveDiscoveryResult(uid)); } catch (Exception e) { return Err(e); } }
}

// ── Sitemaps ──────────────────────────────────────────────────────────────────
[ApiController] [Route("api/sitemaps")]
public class SitemapsController(ClientFactory f) : BaseController(f)
{
    [HttpPost]                   public IActionResult Get([FromBody] OpenHABRequest r)                                                      { try { return Ok(new Sitemaps(Factory.Build(r)).GetSitemaps()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}")]         public IActionResult GetOne([FromRoute] string name, [FromBody] OpenHABRequest r)                          { try { return Ok(new Sitemaps(Factory.Build(r)).GetSitemap(name)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{name}/{page}")]  public IActionResult Page([FromRoute] string name, [FromRoute] string page, [FromBody] OpenHABRequest r)   { try { return Ok(new Sitemaps(Factory.Build(r)).GetSitemapPage(name, page)); } catch (Exception e) { return Err(e); } }
}

// ── Tags / Templates / ModuleTypes / ProfileTypes ─────────────────────────────
[ApiController] [Route("api/tags")]
public class TagsController(ClientFactory f) : BaseController(f)
{
    [HttpPost]               public IActionResult Get([FromBody] OpenHABRequest r)                              { try { return Ok(new Tags(Factory.Build(r)).GetTags()); } catch (Exception e) { return Err(e); } }
    [HttpPost("create")]     public IActionResult Create([FromBody] OpenHABRequest r)                           { try { return Ok(new Tags(Factory.Build(r)).CreateTag(r.Body ?? "")); } catch (Exception e) { return Err(e); } }
    [HttpPost("{id}")]       public IActionResult GetOne([FromRoute] string id, [FromBody] OpenHABRequest r)    { try { return Ok(new Tags(Factory.Build(r)).GetTag(id)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{id}/delete")]public IActionResult Delete([FromRoute] string id, [FromBody] OpenHABRequest r)    { try { return Ok(new Tags(Factory.Build(r)).DeleteTag(id)); } catch (Exception e) { return Err(e); } }
}

[ApiController] [Route("api/templates")]
public class TemplatesController(ClientFactory f) : BaseController(f) {
    [HttpPost] public IActionResult Get([FromBody] OpenHABRequest r) { try { return Ok(new Templates(Factory.Build(r)).GetTemplates()); } catch (Exception e) { return Err(e); } }
}

[ApiController] [Route("api/module-types")]
public class ModuleTypesController(ClientFactory f) : BaseController(f) {
    [HttpPost] public IActionResult Get([FromBody] OpenHABRequest r) { try { return Ok(new ModuleTypes(Factory.Build(r)).GetModuleTypes()); } catch (Exception e) { return Err(e); } }
}

[ApiController] [Route("api/profile-types")]
public class ProfileTypesController(ClientFactory f) : BaseController(f) {
    [HttpPost] public IActionResult Get([FromBody] OpenHABRequest r) { try { return Ok(new ProfileTypes(Factory.Build(r)).GetProfileTypes()); } catch (Exception e) { return Err(e); } }
}

// ── Transformations ───────────────────────────────────────────────────────────
[ApiController] [Route("api/transformations")]
public class TransformationsController(ClientFactory f) : BaseController(f)
{
    [HttpPost]             public IActionResult Get([FromBody] OpenHABRequest r)                              { try { return Ok(new Transformations(Factory.Build(r)).GetTransformations()); } catch (Exception e) { return Err(e); } }
    [HttpPost("services")] public IActionResult Services([FromBody] OpenHABRequest r)                         { try { return Ok(new Transformations(Factory.Build(r)).GetTransformationServices()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{uid}")]    public IActionResult GetOne([FromRoute] string uid, [FromBody] OpenHABRequest r)   { try { return Ok(new Transformations(Factory.Build(r)).GetTransformation(uid)); } catch (Exception e) { return Err(e); } }
}

// ── UI / Services / Iconsets ──────────────────────────────────────────────────
[ApiController] [Route("api/ui")]
public class UIController(ClientFactory f) : BaseController(f)
{
    [HttpPost("tiles")]           public IActionResult Tiles([FromBody] OpenHABRequest r)                              { try { return Ok(new UI(Factory.Build(r)).GetUITiles()); } catch (Exception e) { return Err(e); } }
    [HttpPost("components/{ns}")] public IActionResult Components([FromRoute] string ns, [FromBody] OpenHABRequest r)  { try { return Ok(new UI(Factory.Build(r)).GetUIComponents(ns)); } catch (Exception e) { return Err(e); } }
}

[ApiController] [Route("api/services")]
public class ServicesController(ClientFactory f) : BaseController(f)
{
    [HttpPost]               public IActionResult Get([FromBody] OpenHABRequest r)                              { try { return Ok(new Services(Factory.Build(r)).GetServices()); } catch (Exception e) { return Err(e); } }
    [HttpPost("{id}")]       public IActionResult GetOne([FromRoute] string id, [FromBody] OpenHABRequest r)    { try { return Ok(new Services(Factory.Build(r)).GetService(id)); } catch (Exception e) { return Err(e); } }
    [HttpPost("{id}/config")]public IActionResult Config([FromRoute] string id, [FromBody] OpenHABRequest r)    { try { return Ok(new Services(Factory.Build(r)).GetServiceConfig(id)); } catch (Exception e) { return Err(e); } }
}

[ApiController] [Route("api/iconsets")]
public class IconsetsController(ClientFactory f) : BaseController(f) {
    [HttpPost] public IActionResult Get([FromBody] OpenHABRequest r) { try { return Ok(new Iconsets(Factory.Build(r)).GetIconsets()); } catch (Exception e) { return Err(e); } }
}

// ── Auth ──────────────────────────────────────────────────────────────────────
[ApiController] [Route("api/auth")]
public class AuthController(ClientFactory f) : BaseController(f)
{
    [HttpPost("apitokens")] public IActionResult Tokens([FromBody] OpenHABRequest r)   { try { return Ok(new Auth(Factory.Build(r)).GetAPITokens()); } catch (Exception e) { return Err(e); } }
    [HttpPost("sessions")]  public IActionResult Sessions([FromBody] OpenHABRequest r) { try { return Ok(new Auth(Factory.Build(r)).GetSessions()); } catch (Exception e) { return Err(e); } }
}

// ── Voice ─────────────────────────────────────────────────────────────────────
[ApiController] [Route("api/voice")]
public class VoiceController(ClientFactory f) : BaseController(f)
{
    [HttpPost("voices")]       public IActionResult Voices([FromBody] OpenHABRequest r)       { try { return Ok(new Voice(Factory.Build(r)).GetVoices()); } catch (Exception e) { return Err(e); } }
    [HttpPost("defaultvoice")] public IActionResult DefaultVoice([FromBody] OpenHABRequest r) { try { return Ok(new Voice(Factory.Build(r)).GetDefaultVoice()); } catch (Exception e) { return Err(e); } }
    [HttpPost("interpreters")] public IActionResult Interpreters([FromBody] OpenHABRequest r) { try { return Ok(new Voice(Factory.Build(r)).GetInterpreters()); } catch (Exception e) { return Err(e); } }
    [HttpPost("say")]          public IActionResult Say([FromBody] OpenHABRequest r)          { try { return Ok(new Voice(Factory.Build(r)).SayText(r.Body ?? "", "", "")); } catch (Exception e) { return Err(e); } }
}

// ── SSE Proxy ─────────────────────────────────────────────────────────────────
[ApiController] [Route("api/events")]
public class SSEController(ClientFactory f) : BaseController(f)
{
    [HttpGet("stream")]
    public async Task Stream(
        [FromQuery] string url,
        [FromQuery] string? username,
        [FromQuery] string? password,
        [FromQuery] string? token,
        [FromQuery] string? topics,
        CancellationToken ct)
    {
        Response.Headers["Content-Type"]  = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"]    = "keep-alive";

        var req = new OpenHABRequest { Url = url, Username = username, Password = password, Token = token };
        var client = Factory.Build(req);
        var events = new Events(client);
        var sse = events.GetEvents(topics);

        try
        {
            await foreach (var data in sse.ReadAllAsync(ct))
            {
                await Response.WriteAsync($"data: {data}\n\n", ct);
                await Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException) { }
    }
}
