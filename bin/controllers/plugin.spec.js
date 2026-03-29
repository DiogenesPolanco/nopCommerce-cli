import { jest, describe, test, expect } from '@jest/globals';
import pluginsController from "./plugin.js";

describe('Plugins Controller tests', () => {
    test('pluginsController is defined', () => {
        expect(pluginsController).toBeDefined();
    });
    
    test('pluginsController has create method', () => {
        expect(typeof pluginsController.create).toBe("function");
    });
    
    test('pluginsController has build method', () => {
        expect(typeof pluginsController.build).toBe("function");
    });
    
    test('pluginsController has init method', () => {
        expect(typeof pluginsController.init).toBe("function");
    });
    
    test('pluginsController has config method', () => {
        expect(typeof pluginsController.config).toBe("function");
    });
});
