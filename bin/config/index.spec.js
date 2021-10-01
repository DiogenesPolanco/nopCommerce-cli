import Config from "./index.js";

describe('config tests', () => {
    test('getPath', () => {
        Config.getPath().then((path)=>{
            expect(path).toContain("nopcli");
        });
    });
    test('getGroupAlias', () => {
        expect(Config.getGroupAlias().alias).toBe("group");
    });
    test('getPluginAlias', () => {
        expect(Config.getPluginAlias().alias).toBe("plugin");
    });
    test('getVersionAlias', () => {
        expect(Config.getVersionAlias().alias).toBe("version");
    });
    test('getDescriptionBuildCommand', () => {
        expect(Config.getDescriptionBuildCommand()).toBe("build plugin -[g] -[p]");
    });
    test('getDescriptionBuildCommand', () => {
        expect(Config.getDescriptionNewCommand()).toBe("create plugin -[g] -[p] -[v] -[c] -[b]");
    });
});