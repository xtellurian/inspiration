import { Component, Inject, Input } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from "rxjs/Observable";

@Component({
    selector: 'inspiration-card',
    templateUrl: './inspirationcard.component.html'
})
export class InspirationCardComponent {
    @Input() inspiration: InspirationCard;
    public inspirations: Observable<Array<InspirationCard>>;
}

interface InspirationCard {
    title: string;
    summary: string;
    link: string;
}
