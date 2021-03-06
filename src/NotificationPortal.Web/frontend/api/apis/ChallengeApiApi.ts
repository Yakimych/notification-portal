/* tslint:disable */
/* eslint-disable */
/**
 * NotificationPortal
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import * as runtime from '../runtime';
import {
    ChallengeCollectionModel,
    ChallengeCollectionModelFromJSON,
    ChallengeCollectionModelToJSON,
    SendChallengeModel,
    SendChallengeModelFromJSON,
    SendChallengeModelToJSON,
} from '../models';

export interface ApiChallengesIdGetRequest {
    id: number;
}

export interface ApiChallengesPostRequest {
    sendChallengeModel?: SendChallengeModel;
}

export interface ApiChallengesRpcAcceptIdPostRequest {
    id: number;
}

export interface ApiChallengesRpcDeclineIdPostRequest {
    id: number;
}

/**
 * 
 */
export class ChallengeApiApi extends runtime.BaseAPI {

    /**
     */
    async apiChallengesGetRaw(): Promise<runtime.ApiResponse<ChallengeCollectionModel>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/challenges`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => ChallengeCollectionModelFromJSON(jsonValue));
    }

    /**
     */
    async apiChallengesGet(): Promise<ChallengeCollectionModel> {
        const response = await this.apiChallengesGetRaw();
        return await response.value();
    }

    /**
     */
    async apiChallengesIdGetRaw(requestParameters: ApiChallengesIdGetRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling apiChallengesIdGet.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/challenges/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiChallengesIdGet(requestParameters: ApiChallengesIdGetRequest): Promise<void> {
        await this.apiChallengesIdGetRaw(requestParameters);
    }

    /**
     */
    async apiChallengesPostRaw(requestParameters: ApiChallengesPostRequest): Promise<runtime.ApiResponse<void>> {
        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/api/challenges`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: SendChallengeModelToJSON(requestParameters.sendChallengeModel),
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiChallengesPost(requestParameters: ApiChallengesPostRequest): Promise<void> {
        await this.apiChallengesPostRaw(requestParameters);
    }

    /**
     */
    async apiChallengesRpcAcceptIdPostRaw(requestParameters: ApiChallengesRpcAcceptIdPostRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling apiChallengesRpcAcceptIdPost.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/challenges/rpc/accept/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiChallengesRpcAcceptIdPost(requestParameters: ApiChallengesRpcAcceptIdPostRequest): Promise<void> {
        await this.apiChallengesRpcAcceptIdPostRaw(requestParameters);
    }

    /**
     */
    async apiChallengesRpcDeclineIdPostRaw(requestParameters: ApiChallengesRpcDeclineIdPostRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling apiChallengesRpcDeclineIdPost.');
        }

        const queryParameters: runtime.HTTPQuery = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/challenges/rpc/decline/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiChallengesRpcDeclineIdPost(requestParameters: ApiChallengesRpcDeclineIdPostRequest): Promise<void> {
        await this.apiChallengesRpcDeclineIdPostRaw(requestParameters);
    }

}
