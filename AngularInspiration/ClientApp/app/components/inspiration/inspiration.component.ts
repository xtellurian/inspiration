import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import {InspirationCardComponent} from '../inspirationcard/inspirationcard.component'
import { Observable } from "rxjs/Observable";

@Component({
    selector: 'inspiration',
    templateUrl: './inspiration.component.html',
    styleUrls: ['./inspiration.component.css']
})
export class InspirationComponent {
    public isLoading: boolean;
    public inspiration: InspirationCard;
    public inspirations : InspirationCard[];
    public searchTerm: string;
    private _http: Http;
    private _originUrl: string;
    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        this.isLoading = false;
        this._http = http;
        this._originUrl = originUrl;
        this.inspirations = [];
    };

    public onSubmit = function(){
        if(!this.searchTerm) return;
        this.isLoading = true;
        this._http.get(this._originUrl + '/api/InspirationData/Random?query=' + this.searchTerm + "&num=5&arxiv=true&doaj=true").subscribe(result => {
            var card = result.json() as InspirationCard;
            this.inspiration = card
            this.inspirations.unshift(card);
            this.isLoading = false;
        });
    }
}
    interface InspirationCard {
        title: string;
        summary: string;
        link: string;
    }

