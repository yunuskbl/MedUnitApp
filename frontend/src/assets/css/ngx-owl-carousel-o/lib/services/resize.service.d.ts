import { Observable } from 'rxjs';
import * as i0 from "@angular/core";
export declare class ResizeService {
    private resizeObservable$;
    private docRef;
    /**
     * Makes resizeSubject become Observable
     * @returns Observable of resizeSubject
     */
    get onResize$(): Observable<Event>;
    constructor(winRef: any, docRef: any, platformId: Object);
    static ɵfac: i0.ɵɵFactoryDeclaration<ResizeService, never>;
    static ɵprov: i0.ɵɵInjectableDeclaration<ResizeService>;
}

