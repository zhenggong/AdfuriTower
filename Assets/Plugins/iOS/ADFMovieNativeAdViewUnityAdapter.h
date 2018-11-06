//
//  ADFMovieNativeAdViewUnityAdapter.h
//  Unity-iPhone
//
//  Created by Junhua Li on 2017/04/10.
//
//

#import <Foundation/Foundation.h>
#import <ADFMovieReward/ADFmyMovieNative.h>

@interface ADFMovieNativeAdViewUnityAdapter : NSObject

@property (nonatomic) ADFMovieNativeAdInfo *nativeAdInfo;
@property (nonatomic) ADFMovieNativeAdInfo *oldNativeAdInfo;

- (instancetype)initWithAppID:(NSString *)appID;
- (ADFmyMovieNative *)getMovieNative;
- (void)setNativeAdInformation:(ADFMovieNativeAdInfo *)adInfo;

@end
