export function loadModuleRoutes() {
  const modules = import.meta.glob('../modules/**/routes.js', { eager: true });
  const routes = [];
  Object.keys(modules).forEach((key) => {
    const mod = modules[key];
    if (Array.isArray(mod.default)) {
      routes.push(...mod.default);
    }
    if (Array.isArray(mod.routes)) {
      routes.push(...mod.routes);
    }
  });
  return routes;
}