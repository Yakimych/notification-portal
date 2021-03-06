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
import {
    ChallengeModel,
    ChallengeModelFromJSON,
    ChallengeModelFromJSONTyped,
    ChallengeModelToJSON,
} from './';

/**
 * 
 * @export
 * @interface ChallengeCollectionModel
 */
export interface ChallengeCollectionModel {
    /**
     * 
     * @type {Array<ChallengeModel>}
     * @memberof ChallengeCollectionModel
     */
    challenges: Array<ChallengeModel>;
}

export function ChallengeCollectionModelFromJSON(json: any): ChallengeCollectionModel {
    return ChallengeCollectionModelFromJSONTyped(json, false);
}

export function ChallengeCollectionModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): ChallengeCollectionModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'challenges': ((json['challenges'] as Array<any>).map(ChallengeModelFromJSON)),
    };
}

export function ChallengeCollectionModelToJSON(value?: ChallengeCollectionModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'challenges': ((value.challenges as Array<any>).map(ChallengeModelToJSON)),
    };
}


