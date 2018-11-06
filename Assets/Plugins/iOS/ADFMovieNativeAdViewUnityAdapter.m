//
//  ADFMovieNativeAdViewUnityAdapter.m
//  Unity-iPhone
//
//  Created by Junhua Li on 2017/04/10.
//
//

#import "ADFMovieNativeAdViewUnityAdapter.h"
#import <ADFMovieReward/ADFmyMovieNative.h>

@interface ADFMovieNativeAdViewUnityAdapter()
@property (nonatomic) ADFmyMovieNative *movieNative;
@property (nonatomic, copy) NSString* appID;
@end

@implementation ADFMovieNativeAdViewUnityAdapter

- (instancetype)initWithAppID:(NSString *)appID {
    self = [super init];
    if (self) {
        _movieNative = [ADFmyMovieNative getInstance:appID];
        _appID = [[NSString alloc] initWithString:appID];
    }
    return self;
}

- (ADFmyMovieNative *)getMovieNative {
    return self.movieNative;
}

- (void)setNativeAdInformation:(ADFMovieNativeAdInfo *)adInfo {
    if (adInfo) {
        if (self.nativeAdInfo.mediaView.superview) {
            self.oldNativeAdInfo = self.nativeAdInfo;
        }
    } else {
        self.oldNativeAdInfo = nil;
    }
    self.nativeAdInfo = adInfo;
}

@end
