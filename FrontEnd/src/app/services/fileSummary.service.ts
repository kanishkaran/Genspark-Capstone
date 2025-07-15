import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environment/environment";



@Injectable()
export class FileSummaryService{
    private http = inject(HttpClient);
    private readonly apiUrl = environment.apiUrl;
    readonly endpoint = 'FileSummary'

    getFileSummary(fileName: string){
        let params = new HttpParams().set('fileName', fileName);
        return this.http.get(`${this.apiUrl}/${this.endpoint}/summarize/`, {
            params
        })
    }

    semanticSearch(query: any){
        console.log(query)
        let params = new HttpParams().set('query',query)
        return this.http.get(`${this.apiUrl}/${this.endpoint}/`, {params})
    }
}