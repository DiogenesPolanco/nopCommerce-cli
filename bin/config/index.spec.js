import {config} from "./index.js";

describe('config tests', () => {
    test('getPath', () => {
        config.getPath().then((path)=>{
            expect(path).toContain("nopcli");
        });
    });
    test('getGroupAlias', () => {
        expect(config.getGroupAlias().alias).toBe("group");
    });
    test('getPluginAlias', () => {
        expect(config.getPluginAlias().alias).toBe("plugin");
    });
    test('getVersionAlias', () => {
        expect(config.getVersionAlias().alias).toBe("version");
    });
    test('getDescriptionBuildCommand', () => {
        expect(config.getDescriptionBuildCommand()).toBe("build plugin -[g] -[p]");
    });
    test('getDescriptionBuildCommand', () => {
        expect(config.getDescriptionNewCommand()).toBe("create plugin -[g] -[p] -[v] -[c]");
    });
});