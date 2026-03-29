import { jest, describe, test, expect } from '@jest/globals';
import Config from "./index.js";

describe('config tests', () => {
    test('getGroupAlias', () => {
        expect(Config.getGroupAlias().alias).toBe("group");
    });
    
    test('getGroupAlias default', () => {
        expect(Config.getGroupAlias().default).toBe("Widgets");
    });
    
    test('getPluginAlias', () => {
        expect(Config.getPluginAlias().alias).toBe("plugin");
    });
    
    test('getVersionAlias', () => {
        expect(Config.getVersionAlias().alias).toBe("version");
    });
    
    test('getVersionAlias default', () => {
        expect(Config.getVersionAlias().default).toBe(460);
    });
    
    test('getDescriptionBuildCommand', () => {
        expect(Config.getDescriptionBuildCommand()).toBe("build plugin -[g] -[p]");
    });
    
    test('getDescriptionNewCommand', () => {
        expect(Config.getDescriptionNewCommand()).toBe("create plugin -[g] -[p] -[v] -[c] -[b]");
    });
    
    test('getDescriptionInitCommand', () => {
        expect(Config.getDescriptionInitCommand()).toBe("init plugin -[g] -[p] -[v] -[b]");
    });
    
    test('getClientSettingFileName', () => {
        expect(Config.getClientSettingFileName()).toBe(".nopcli");
    });
    
    test('getCloneNopDefaultCommand contains branch', () => {
        const cmd = Config.getCloneNopDefaultCommand();
        expect(cmd).toContain("git clone");
        expect(cmd).toContain("nopCommerce");
    });
    
    test('getConfigPluginQuestions returns array', () => {
        const questions = Config.getConfigPluginQuestions();
        expect(Array.isArray(questions)).toBe(true);
        expect(questions.length).toBeGreaterThan(0);
    });
    
    test('Prepare with empty args returns defaults', () => {
        const args = {};
        const result = Config.Prepare('.', args);
        expect(result.g).toBeDefined();
        expect(result.v).toBeDefined();
    });
});
