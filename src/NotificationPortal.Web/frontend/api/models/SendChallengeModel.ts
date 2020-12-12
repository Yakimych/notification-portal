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

import { exists, mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface SendChallengeModel
 */
export interface SendChallengeModel {
    /**
     * 
     * @type {string}
     * @memberof SendChallengeModel
     */
    communityName: string;
    /**
     * 
     * @type {string}
     * @memberof SendChallengeModel
     */
    fromPlayer: string;
    /**
     * 
     * @type {string}
     * @memberof SendChallengeModel
     */
    toPlayer: string;
    /**
     * 
     * @type {string}
     * @memberof SendChallengeModel
     */
    requestStatusMessage?: string | null;
}

export function SendChallengeModelFromJSON(json: any): SendChallengeModel {
    return SendChallengeModelFromJSONTyped(json, false);
}

export function SendChallengeModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): SendChallengeModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'communityName': json['communityName'],
        'fromPlayer': json['fromPlayer'],
        'toPlayer': json['toPlayer'],
        'requestStatusMessage': !exists(json, 'requestStatusMessage') ? undefined : json['requestStatusMessage'],
    };
}

export function SendChallengeModelToJSON(value?: SendChallengeModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'communityName': value.communityName,
        'fromPlayer': value.fromPlayer,
        'toPlayer': value.toPlayer,
        'requestStatusMessage': value.requestStatusMessage,
    };
}

