import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';

import { Observable } from 'rxjs/Observable';

// Import RxJs required methods
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable()
export class AppService {
    private apiUrl = 'api/account/transfer';
    constructor(private _http: Http) { }

    postData(data: any): Observable<any> {
        return this._http.post(this.apiUrl, data).map((response: Response) => {
            return response.text();
        });
    }
}
